using Hangfire;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
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
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class ImportSalesOrderCommandHandler
        : IRequestHandler<ImportSaleOrderCommand, ImportSaleOrderResult>
    {
        private readonly ILogger<ImportSalesOrderCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly SalesOrderValidator salesOrderValidator;
        private readonly ISalesOrderRepository salesOrderRepository;
        private readonly ISalesContractDetailRepository salesContractDetailRepository;
        private readonly IPurchaseOrderTypeRepository purchaseOrderTypeRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IPartRepository partRepository;
        private readonly ISizeRepository sizeRepository;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly IEntitySequenceNumberRepository entitySequenceNumberRepository;
        private readonly IMediator mediator;

        public ImportSalesOrderCommandHandler(ILogger<ImportSalesOrderCommandHandler> logger,
            SqlServerAppDbContext context,
            SalesOrderValidator salesOrderValidator,
            ISalesOrderRepository salesOrderRepository,
            ISalesContractDetailRepository salesContractDetailRepository,
            IPurchaseOrderTypeRepository purchaseOrderTypeRepository,
            ICustomerRepository customerRepository,
            IPartRepository partRepository,
            ISizeRepository sizeRepository,
            IItemStyleRepository itemStyleRepository,
            IEntitySequenceNumberRepository entitySequenceNumberRepository,
            IMediator mediator)
        {
            this.logger = logger;
            this.context = context;
            this.salesOrderValidator = salesOrderValidator;
            this.salesOrderRepository = salesOrderRepository;
            this.salesContractDetailRepository = salesContractDetailRepository;
            this.purchaseOrderTypeRepository = purchaseOrderTypeRepository;
            this.customerRepository = customerRepository;
            this.partRepository = partRepository;
            this.sizeRepository = sizeRepository;
            this.itemStyleRepository = itemStyleRepository;
            this.entitySequenceNumberRepository = entitySequenceNumberRepository;
            this.mediator = mediator;
        }

        public async Task<ImportSaleOrderResult> Handle(ImportSaleOrderCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute import sales order command", DateTime.Now.ToString());
            var result = new ImportSaleOrderResult();
            string fileName = request.File.FileName;

            if (salesOrderRepository.ExistFileSalesOrders(fileName))
            {
                result.Message = "File has exist";
                return result;
            }

            if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }
            var salesContractDetails = salesContractDetailRepository.GetSalesContractDetails();
            var parts = partRepository.GetParts(request.CustomerID);
            var sizes = sizeRepository.GetSizes(request.CustomerID);
            var purchaseOrderTypes = purchaseOrderTypeRepository.GetPurchaseOrderTypes();
            List<SalesOrder> salesOrderList;
            var salesOrder = ImportSalesOrderProcess.Import(fullPath, fileName, request.GetUser(), request.CustomerID, sizes,
                                                            salesContractDetails, itemStyleRepository.GetItemStyles(request.CustomerID),
                                                            purchaseOrderTypes.ToList(),
                                                            parts, out List<Part> newParts,
                                                            out List<PurchaseOrderType> newPurchaseOrderTypes,
                                                            out salesOrderList, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            var customer = customerRepository.GetCustomer(request.CustomerID);

            if (customer != null)
            {
                salesOrder.PaymentTermCode = customer.PaymentTerm?.Code;
                salesOrder.PaymentTermDescription = customer.PaymentTerm?.Description;

                salesOrder.CurrencyID = customer.Currency?.ID;
                salesOrder.CustomerName = customer.Name;

                salesOrder.PriceTermDescription = customer.PriceTerm?.Description;
                salesOrder.PriceTermCode = customer.PriceTerm?.Code;

                salesOrder.BrandCode = request.BrandCode;
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            if (!salesOrderValidator.IsValid(salesOrder, out errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            var sequenceNum = entitySequenceNumberRepository
                .GetNextNumberByCode(nameof(SalesOrder), out EntitySequenceNumber sequenceNumber);

            salesOrder.ID += sequenceNum;
            salesOrder.SetCreateAudit(request.GetUser());

            try
            {
                switch (request.CustomerID)
                {
                    case "HA":
                        if (salesOrderList.Any())
                        {
                            context.SalesOrders.AddRange(salesOrderList);
                        }

                        break;
                    default:
                        context.SalesOrders.Add(salesOrder);
                        break;
                }

                context.Part.AddRange(newParts);
                context.Part.UpdateRange(parts);
                context.EntitySequenceNumber.Update(sequenceNumber);
                context.PurchaseOrderType.AddRange(newPurchaseOrderTypes);

                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;

                result.Result = salesOrder;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }
            finally
            {
                await mediator.Publish(new ItemStyleBulkedEvent()
                {
                    ItemStyleNumbers = salesOrder.ItemStyles.Select(x => x.Number).ToList(),
                    UserName = request.Username
                });

                var jobId = BackgroundJob.Enqueue<UpdateShipQuantityJob>(j => j.Execute(salesOrder.ItemStyles.Select(x => x.Number).ToList()));
            }

            return result;
        }
    }
}
