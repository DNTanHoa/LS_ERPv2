using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class CreateReceiptFabricCommandHandler
        : IRequestHandler<CreateReceiptFabricCommand, CommonCommandResult<Receipt>>
    {
        private readonly ILogger<CreateReceiptFabricCommandHandler> logger;
        private readonly IReceiptRepository receiptRepository;
        private readonly IEntitySequenceNumberRepository entitySequenceNumberRepository;
        private readonly IStorageDetailRepository storageDetailRepository;
        private readonly IFabricPurchaseOrderRepository fabricPurchaseOrderRepository;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;

        public CreateReceiptFabricCommandHandler(ILogger<CreateReceiptFabricCommandHandler> logger,
            IReceiptRepository receiptRepository,
            IEntitySequenceNumberRepository entitySequenceNumberRepository,
            IStorageDetailRepository storageDetailRepository,
            IFabricPurchaseOrderRepository fabricPurchaseOrderRepository,
            IMapper mapper,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.receiptRepository = receiptRepository;
            this.entitySequenceNumberRepository = entitySequenceNumberRepository;
            this.storageDetailRepository = storageDetailRepository;
            this.fabricPurchaseOrderRepository = fabricPurchaseOrderRepository;
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<CommonCommandResult<Receipt>> Handle(CreateReceiptFabricCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("--- Execute create receipt fabric command at {@time} with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(request));
            LogHelper.Instance.Information("--- Execute create receipt fabric command at {@time} with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(request));

            var result = new CommonCommandResult<Receipt>();

            var receipt = mapper.Map<Receipt>(request);
            receipt.Number = entitySequenceNumberRepository.GetNextNumberByCode("Receipt",
                out EntitySequenceNumber sequenceNumber);
            receipt.SetCreateAudit(request.Username);

            //using var transaction = context.Database.BeginTransaction();
            //transaction.CreateSavepoint("SaveChange");

            /// CHECK HB
            var checkHB = false;
            if (!string.IsNullOrEmpty(request.InvoiceNumber) && request.InvoiceNumber.ToUpper().Contains("HB")
                && !request.FabricPurchaseOrderNumber.ToUpper().Contains("HB"))
            {
                checkHB = true;
                receipt.FabricPurchaseOrderNumber = "HB-" + receipt.FabricPurchaseOrderNumber;
                request.FabricPurchaseOrderNumber = receipt.FabricPurchaseOrderNumber;

                FabricPurchaseOrder oldFabricPurchaseOrder = fabricPurchaseOrderRepository.GetFabricPurchaseOrder(request.FabricPurchaseOrderID);

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<FabricPurchaseOrder, FabricPurchaseOrder>()
                        .ForMember(x => x.ID, y => y.Ignore())
                        .ForMember(x => x.Number, y => y.Ignore())
                        .ForMember(x => x.Unit, y => y.Ignore())
                        .ForMember(x => x.Customer, y => y.Ignore());
                });

                var mapper = config.CreateMapper();
                var newFabric = mapper.Map<FabricPurchaseOrder>(oldFabricPurchaseOrder);

                newFabric.ProductionMethodCode = request.ProductionMethodCode;
                newFabric.Number = receipt.FabricPurchaseOrderNumber;

                if (newFabric.ReceivedQuantity == null)
                    newFabric.ReceivedQuantity = 0;

                if (newFabric.OnHandQuantity == null)
                {
                    newFabric.OnHandQuantity = 0;
                }

                if (newFabric.DeliveredQuantity == null)
                    newFabric.DeliveredQuantity = 0;

                if (newFabric.ShippedQuantity == null)
                {
                    newFabric.ShippedQuantity = 0;
                }

                newFabric.ReceivedQuantity = 0;
                newFabric.OnHandQuantity = 0;
                newFabric.DeliveredQuantity = 0;
                newFabric.ShippedQuantity = 0;

                var receivedQty = receipt.ReceiptGroupLines
                    .Where(r => r.FabricPurchaseOrderNumber == newFabric.Number)
                    .Sum(s => s.ReceiptQuantity);

                newFabric.ReceivedQuantity += receivedQty;

                newFabric.ShippedQuantity = receivedQty;
                newFabric.DeliveredQuantity = receivedQty;
                newFabric.OnHandQuantity = receivedQty + newFabric.OnHandQuantity;

                context.FabricPurchaseOrder.Add(newFabric);
                context.SaveChanges();

                receipt.FabricPurchaseOrderID = newFabric.ID;
                foreach (var item in receipt.ReceiptGroupLines)
                {
                    item.FabricPurchaseOrderGroupLineID = receipt.FabricPurchaseOrderID;
                }
            }

            ///Create material transactions
            var transactions = MaterialTransactionProcess.CreateTransactions(receipt, request.GetUser());

            ///Caculate and update storage detail
            var storageDetails = storageDetailRepository.GetFabricStorageDetails(request.FabricPurchaseOrderNumber);
            StorageDetailProcess.UpdateStorageFromReceipt(storageDetails?.ToList(), transactions, request.StorageCode,
                request.GetUser(),
                out List<StorageDetail> newStorageDetail);

            ///Update Fabric purchase order information data
            if (!checkHB)
            {
                FabricPurchaseOrder fabricPurchaseOrder = fabricPurchaseOrderRepository.GetFabricPurchaseOrder(receipt.FabricPurchaseOrderID);

                ImportFabricPurchaseOrderProcessor.CalculateReceiptQuantity(fabricPurchaseOrder, receipt.ReceiptGroupLines.ToList(), out string errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    result.Message = "Error saving file";
                    return result;
                }

                context.FabricPurchaseOrder.Update(fabricPurchaseOrder);

            }

            receiptRepository.Add(receipt);

            try
            {

                context.EntitySequenceNumber.UpdateRange(sequenceNumber);
                context.MaterialTransaction.AddRange(transactions);
                context.StorageDetail.AddRange(newStorageDetail);
                context.StorageDetail.UpdateRange(storageDetails);

                await context.SaveChangesAsync();

                //transaction.Commit();
            }
            catch (DbUpdateException ex)
            {
                //transaction.RollbackToSavepoint("SaveChange");
                LogHelper.Instance.Error("--- Execute create receipt fabric command at {@time} fail with message {@request}",
                    DateTime.Now.ToString(), ex.InnerException.Message);
                logger.LogError(ex.InnerException.Message);
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
