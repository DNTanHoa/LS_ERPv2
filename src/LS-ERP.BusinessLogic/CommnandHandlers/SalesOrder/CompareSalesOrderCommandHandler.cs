using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Global;
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
    public class CompareSalesOrderCommandHandler
        : IRequestHandler<CompareDataCommand, CompareDataResult>
    {
        private readonly ILogger<CompareSalesOrderCommandHandler> logger;
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

        public CompareSalesOrderCommandHandler(ILogger<CompareSalesOrderCommandHandler> logger,
            SqlServerAppDbContext context,
            ISalesOrderRepository salesOrderRepository,
            SalesOrderValidator salesOrderValidator,
            ICustomerRepository customerRepository,
            IItemStyleRepository itemStyleRepository,
            ISalesContractDetailRepository salesContractDetailRepository,
            IPartRepository partRepository,
            IPurchaseOrderTypeRepository purchaseOrderTypeRepository,
            ISizeRepository sizeRepository,
            IEntitySequenceNumberRepository entitySequenceNumberRepository)
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
        }

        public async Task<CompareDataResult> Handle(CompareDataCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute compare sales order command", DateTime.Now.ToString());
            var result = new CompareDataResult();
            string fileName = request.CompareFile.FileName;

            if (salesOrderRepository.ExistFileSalesOrders(fileName))
            {
                result.Message = "File has exist";
                return result;
            }

            if (!FileHelpers.SaveFile(request.CompareFile, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var salesContractDetails = salesContractDetailRepository.GetSalesContractDetails();
            //var salesOrders = salesOrderRepository.GetSalesOrders(request.CustomerID);
            var parts = partRepository.GetParts(request.CustomerID);
            var sizes = sizeRepository.GetSizes(request.CustomerID);
            var purchaseOrderTypes = purchaseOrderTypeRepository.GetPurchaseOrderTypes();
            List<SalesOrder> salesOrderList;
            bool isNotCompare = false;
            ImportSalesOrderProcess.Import(fullPath, fileName, request.GetUser(), request.CustomerID, sizes,
                                                            salesContractDetails, itemStyleRepository.GetItemStyles(request.CustomerID),
                                                            purchaseOrderTypes.ToList(),
                                                            parts, out List<Part> newParts,
                                                            out List<PurchaseOrderType> newPurchaseOrderTypes,
                                                            out salesOrderList, out string errorMessage,
                                                            isNotCompare);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            List<ItemStyle> itemStyles = new List<ItemStyle>();
            string salesOrders = string.Empty;
            List<string> salesOrderIDs = new List<string>();
            foreach (var itemSalesOrder in salesOrderList)
            {
                salesOrders += itemSalesOrder.ID + ";";
                salesOrderIDs.Add(itemSalesOrder.ID);
            }

            //var oldItemStyle = itemStyleRepository.GetItemStylesFollowSalesOrderID(salesOrderIDs);
            var oldItemStyle = itemStyleRepository.GetItemStylesForCompareData(salesOrderIDs);
            if (oldItemStyle != null && oldItemStyle.Count() > 0)
            {
                foreach (var item in oldItemStyle)
                {
                    if (item != null)
                        itemStyles.Add(item);
                }
            }

            List<SalesOrderCompareDto> compareSalesOrderDto = new List<SalesOrderCompareDto>();
            CompareSalesOrderDataProcess.CreateCompareItemList(salesOrderList, itemStyles,
                                            request.FromDate, ref compareSalesOrderDto, out string message);

            if (!string.IsNullOrEmpty(message))
            {
                result.Message = message;
                return result;
            }

            try
            {
                result.IsSuccess = true;
                GroupCompareDto groupCompare = new GroupCompareDto
                {
                    CompareItemStyles = compareSalesOrderDto,
                    SalesOrders = salesOrders,
                    FileName = fileName,
                    FilePath = fullPath
                };
                result.GroupCompare = groupCompare;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
