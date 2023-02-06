using AutoMapper;
using Hangfire;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Events;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.BusinessLogic.Shared.Process.Jobs;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Global;
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
    public class UpdateSalesOrderCommandHandler
        : IRequestHandler<UpdateSalesOrderCommand, UpdateSalesOrderResult>
    {
        private readonly ILogger<UpdateSalesOrderCommandHandler> logger;
        private readonly SalesOrderValidator salesOrderValidator;
        private readonly ISalesOrderRepository salesOrderRepository;
        private readonly ISizeRepository sizeRepository;
        private readonly IPartRepository partRepository;
        private readonly IPurchaseOrderTypeRepository purchaseOrderTypeRepository;
        private readonly IJobHeadRepository jobHeadRepository;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly IMediator mediator;
        private readonly SqlServerAppDbContext context;

        public UpdateSalesOrderCommandHandler(ILogger<UpdateSalesOrderCommandHandler> logger,
            SalesOrderValidator salesOrderValidator,
            ISalesOrderRepository salesOrderRepository,
            ISizeRepository sizeRepository,
            IPartRepository partRepository,
            IPurchaseOrderTypeRepository purchaseOrderTypeRepository,
            IJobHeadRepository jobHeadRepository,
            IItemStyleRepository itemStyleRepository,
            IMediator mediator,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.salesOrderValidator = salesOrderValidator;
            this.salesOrderRepository = salesOrderRepository;
            this.sizeRepository = sizeRepository;
            this.partRepository = partRepository;
            this.purchaseOrderTypeRepository = purchaseOrderTypeRepository;
            this.jobHeadRepository = jobHeadRepository;
            this.itemStyleRepository = itemStyleRepository;
            this.mediator = mediator;
            this.context = context;
        }

        public async Task<UpdateSalesOrderResult> Handle(UpdateSalesOrderCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute update sales order command", DateTime.Now.ToString());
            var result = new UpdateSalesOrderResult();
            string fileName = request.UpdateFile.FileName;

            if (!FileHelpers.SaveFile(request.UpdateFile, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var existSalesOrder = salesOrderRepository.GetSalesOrder(request.ID);

            if (existSalesOrder == null)
            {
                result.Message = "Not found sale order";
                return result;
            }

            List<string> oldSalesOrderList = new List<string>();
            oldSalesOrderList.Add(existSalesOrder.ID);

            var parts = partRepository.GetParts(request.CustomerID);
            var sizes = sizeRepository.GetSizes(request.CustomerID);

            var purchaseOrderTypes = purchaseOrderTypeRepository.GetPurchaseOrderTypes();

            UpdateSalesOrderProcess.UpdateSalesOrder(
                existSalesOrder, request.Username, fullPath, fileName, request.CustomerID, sizes,
                purchaseOrderTypes.ToList(), parts, itemStyleRepository.GetItemStyles(request.CustomerID),
                out List<Part> newParts,
                out List<PurchaseOrderType> newPurchaseOrderTypes,
                out List<OrderDetail> orderDetails,
                out List<OrderDetail> newOrderDetails,
                out List<ItemStyle> newItemStyle,
                out List<ItemStyle> updateItemStyle,
                out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            if (!salesOrderValidator.IsValid(existSalesOrder, out errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            existSalesOrder.BrandCode = request.BrandCode;
            existSalesOrder.ConfirmDate = request.ConfirmDate;
            existSalesOrder.CurrencyID = request.CurrencyID;
            existSalesOrder.DivisionID = request.DivisionID;
            existSalesOrder.OrderDate = request.OrderDate;
            existSalesOrder.PaymentTermCode = request.PaymentTermCode;
            existSalesOrder.PriceTermCode = request.PriceTermCode;
            existSalesOrder.Year = request.Year;
            existSalesOrder.SalesOrderStatusCode = request.SalesOrderStatusCode;
            existSalesOrder.SalesOrderTypeCode = request.SalesOrderOrderTypeCode;
            existSalesOrder.SetUpdateAudit(request.GetUser());
            existSalesOrder.FileName = fileName;
            existSalesOrder.SaveFilePath = fullPath;

            try
            {
                if (newPurchaseOrderTypes.Any())
                {
                    context.PurchaseOrderType.AddRange(newPurchaseOrderTypes);
                }

                if (updateItemStyle.Any())
                {
                    updateItemStyle.ForEach(x =>
                    {
                        x.PurchaseOrderType = null;
                        x.PurchaseOrderStatus = null;
                    });

                    context.ItemStyle.UpdateRange(updateItemStyle);
                }

                if (newItemStyle.Any())
                {
                    context.ItemStyle.AddRange(newItemStyle);
                }

                if (newOrderDetails.Any())
                {
                    context.OrderDetail.AddRange(newOrderDetails);
                }

                context.SalesOrders.Update(existSalesOrder);
                context.SaveChanges();
                result.IsSuccess = true;
                result.Result = existSalesOrder;

                foreach (var item in newOrderDetails)
                {
                    orderDetails.Add(item);
                }

                await mediator.Publish(new OrderDetailBulkUpdatedEvent()
                {
                    UpdatedOrderDetails = orderDetails,
                    UserName = request.Username
                });
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }
            finally
            {
                var jobId = BackgroundJob.Enqueue<UpdateShipQuantityJob>(j => j.Execute(existSalesOrder.ItemStyles.Select(x => x.Number).ToList()));
            }

            return result;
        }
    }
}
