using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Events;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.XAF.Module.Dtos.SalesOrder;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class SaveCompareSalesOrderCommandHandler
        : IRequestHandler<SaveCompareCommand, CompareDataResult>
    {
        private readonly ILogger<SaveCompareSalesOrderCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly ISalesOrderRepository salesOrderRepository;
        private readonly SalesOrderValidator salesOrderValidator;
        private readonly ICustomerRepository customerRepository;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly ISalesContractDetailRepository salesContractDetailRepository;
        private readonly IPartRepository partRepository;
        private readonly IPurchaseOrderTypeRepository purchaseOrderTypeRepository;
        private readonly ISizeRepository sizeRepository;
        private readonly IEntitySequenceNumberRepository entitySequenceNumberRepository;
        private readonly IJobHeadRepository jobHeadRepository;
        private readonly ILabelPortRepository labelPortRepository;
        private readonly IMediator mediator;

        public SaveCompareSalesOrderCommandHandler(ILogger<SaveCompareSalesOrderCommandHandler> logger,
            SqlServerAppDbContext context,
            ISalesOrderRepository salesOrderRepository,
            SalesOrderValidator salesOrderValidator,
            ICustomerRepository customerRepository,
            IItemStyleRepository itemStyleRepository,
            ISalesContractDetailRepository salesContractDetailRepository,
            IPartRepository partRepository,
            IPurchaseOrderTypeRepository purchaseOrderTypeRepository,
            ISizeRepository sizeRepository,
            IEntitySequenceNumberRepository entitySequenceNumberRepository,
            IJobHeadRepository jobHeadRepository,
            ILabelPortRepository labelPortRepository,
            IMediator mediator)
        {
            this.logger = logger;
            this.context = context;
            this.salesOrderRepository = salesOrderRepository;
            this.salesOrderValidator = salesOrderValidator;
            this.customerRepository = customerRepository;
            this.itemStyleRepository = itemStyleRepository;
            this.salesContractDetailRepository = salesContractDetailRepository;
            this.partRepository = partRepository;
            this.purchaseOrderTypeRepository = purchaseOrderTypeRepository;
            this.sizeRepository = sizeRepository;
            this.entitySequenceNumberRepository = entitySequenceNumberRepository;
            this.jobHeadRepository = jobHeadRepository;
            this.labelPortRepository = labelPortRepository;
            this.mediator = mediator;
        }
        public async Task<CompareDataResult> Handle(SaveCompareCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute compare sales order command", DateTime.Now.ToString());
            var result = new CompareDataResult();
            var strSalesOrders = request.GroupCompare?.SalesOrders?.Substring(0, (int)request.GroupCompare?.SalesOrders?.Length - 1);
            var arrSalesOrders = strSalesOrders.Split(";");
            List<string> oldSalesOrderList = new List<string>();
            List<string> newSalesOrderList = new List<string>();

            var parts = partRepository.GetParts(request.CustomerID).ToList();
            var sizes = sizeRepository.GetSizes(request.CustomerID);

            foreach (var salesOrderID in arrSalesOrders)
            {
                if (salesOrderRepository.IsExist(salesOrderID))
                {
                    oldSalesOrderList.Add(salesOrderID);
                }
                else
                {
                    newSalesOrderList.Add(salesOrderID);
                }
            }
            var oldItemStyles = itemStyleRepository.GetItemStylesFollowSalesOrderID(oldSalesOrderList);
            var oldSalesOrders = salesOrderRepository.GetSalesOrders(oldSalesOrderList);
            var dicLabelPorts = labelPortRepository.GetLabelPorts(request.CustomerID).ToDictionary(x => x.Division + x.LabelCode);

            //var jobhead = jobHeadRepository.GetJobHeads(oldItemStyles.Select(x => x.CustomerStyle).ToList()).ToList();
            //var salesContractDetails = salesContractDetailRepository.GetSalesContractDetails();
            CompareSalesOrderDataProcess.CompareItemStyle(request.Username, request.CustomerID, request.GroupCompare.FilePath,
                                                          request.GroupCompare.FileName, oldItemStyles.ToList(),
                                                          request.GroupCompare.CompareItemStyles, newSalesOrderList,
                                                          ref parts, sizes,
                                                          out List<Part> newParts,
                                                          out List<SalesOrder> newSalesOrders,
                                                          out List<ItemStyle> newItemStyles,
                                                          out List<ItemStyle> updateItemStyles,
                                                          out List<JobHead> updateJobHeads,
                                                          out List<OrderDetail> orderDetails,
                                                          out string errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            try
            {
                foreach (var salesOrder in oldSalesOrders)
                {
                    salesOrder.FileName = request.GroupCompare.FileName;
                    salesOrder.SaveFilePath = request.GroupCompare.FilePath;
                    salesOrder.SetUpdateAudit(request.Username);
                }

                if (newSalesOrders.Count > 0)
                {
                    foreach (var salesOrder in newSalesOrders)
                    {
                        salesOrder.FileName = request.GroupCompare.FileName;
                        salesOrder.SaveFilePath = request.GroupCompare.FilePath;
                    }
                    context.SalesOrders.AddRange(newSalesOrders);
                }

                if (dicLabelPorts != null)
                {
                    foreach (var itemStyle in newItemStyles)
                    {
                        string key = itemStyle.Division + itemStyle.LabelCode;
                        if (dicLabelPorts.TryGetValue(key, out LabelPort rsLabelPort))
                        {
                            itemStyle.ETAPort = rsLabelPort.ETAPort;
                        }
                        else if (!int.TryParse(itemStyle.Division, out int division))
                        {
                            itemStyle.ETAPort = "NEW JERSEY";
                        }
                    }

                    foreach (var itemStyle in updateItemStyles)
                    {
                        string key = itemStyle.Division + itemStyle.LabelCode;
                        if (dicLabelPorts.TryGetValue(key, out LabelPort rsLabelPort))
                        {
                            itemStyle.ETAPort = rsLabelPort.ETAPort;
                        }
                        else if (!int.TryParse(itemStyle.Division, out int division))
                        {
                            itemStyle.ETAPort = "NEW JERSEY";
                        }
                    }
                }


                context.Part.AddRange(newParts);
                context.Part.UpdateRange(parts);

                context.ItemStyle.AddRange(newItemStyles);
                context.ItemStyle.UpdateRange(updateItemStyles);

                context.JobHead.UpdateRange(updateJobHeads);

                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.GroupCompare = request.GroupCompare;

                await mediator.Publish(new OrderDetailBulkUpdatedEvent()
                {
                    UpdatedOrderDetails = orderDetails,
                    UserName = request.Username
                });

            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
