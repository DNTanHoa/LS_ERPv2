using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Events;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
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
    public class CreateReceiptCommandHandler
        : IRequestHandler<CreateReceiptCommand, CommonCommandResult<Receipt>>
    {
        private readonly ILogger<CreateReceiptCommandHandler> logger;
        private readonly IReceiptRepository receiptRepository;
        private readonly IEntitySequenceNumberRepository entitySequenceNumberRepository;
        private readonly IStorageDetailRepository storageDetailRepository;
        private readonly IPurchaseOrderGroupLineRepository purchaseOrderGroupLineRepository;
        private readonly IFabricPurchaseOrderRepository fabricPurchaseOrderRepository;
        private readonly IPurchaseOrderLineRepository purchaseOrderLineRepository;
        private readonly IShipmentDetailRepository shipmentDetailRepository;
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        private readonly SqlServerAppDbContext context;

        public CreateReceiptCommandHandler(ILogger<CreateReceiptCommandHandler> logger,
            IReceiptRepository receiptRepository,
            IEntitySequenceNumberRepository entitySequenceNumberRepository,
            IStorageDetailRepository storageDetailRepository,
            IPurchaseOrderGroupLineRepository purchaseOrderGroupLineRepository,
            IFabricPurchaseOrderRepository fabricPurchaseOrderRepository,
            IPurchaseOrderLineRepository purchaseOrderLineRepository,
            IShipmentDetailRepository shipmentDetailRepository,
            IMapper mapper,
            IMediator mediator,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.receiptRepository = receiptRepository;
            this.entitySequenceNumberRepository = entitySequenceNumberRepository;
            this.storageDetailRepository = storageDetailRepository;
            this.purchaseOrderGroupLineRepository = purchaseOrderGroupLineRepository;
            this.fabricPurchaseOrderRepository = fabricPurchaseOrderRepository;
            this.purchaseOrderLineRepository = purchaseOrderLineRepository;
            this.shipmentDetailRepository = shipmentDetailRepository;
            this.mapper = mapper;
            this.mediator = mediator;
            this.context = context;
        }
        public async Task<CommonCommandResult<Receipt>> Handle(CreateReceiptCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("--- Execute create receipt command at {@time} with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(request));
            LogHelper.Instance.Information("--- Execute create receipt command at {@time} with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(request));

            var result = new CommonCommandResult<Receipt>();

            var receipt = mapper.Map<Receipt>(request);
            receipt.Number = entitySequenceNumberRepository.GetNextNumberByCode("Receipt",
                out EntitySequenceNumber sequenceNumber);
            receipt.SetCreateAudit(request.Username);

            ///Create material transactions
            ///var transactions = MaterialTransactionProcess.CreateTransactions(receipt, request.GetUser());

            ///Caculate and update storage detail
            ///var storageDetails = storageDetailRepository.GetStorageDetails(receipt.StorageCode,
            ///    receipt.PurchaseOrder?.Number);
            ///StorageDetailProcess.UpdateStorageFromReceipt(storageDetails.ToList(), transactions, request.StorageCode,
            ///    request.GetUser(),
            ///    out List<StorageDetail> newStorageDetail);

            ///Update purchase order information data
            ///var purchaseOrderGroupLineIDs = receipt.ReceiptGroupLines
            ///    .Where(r => r.PurchaseOrderGroupLineID != null)
            ///    .Select(x => (long)x.PurchaseOrderGroupLineID).Distinct().ToList();
            ///var purchaseOrderGroupLines = purchaseOrderGroupLineRepository
            ///    .GetPurchaseOrderGroupLines(purchaseOrderGroupLineIDs);

            ///UpdatePurchaseOrderGroupLineQuantityProcess.CalculateReceiptQuantity(purchaseOrderGroupLines.ToList(),
            ///    receipt.ReceiptGroupLines.ToList(),
            ///    out string errorMessage);

            ///if (!string.IsNullOrEmpty(errorMessage))
            ///{
            ///    result.Message = "Error saving processs";
            ///    return result;
            ///}

            ///receiptRepository.Add(receipt);

            try
            {
                context.Receipt.Add(receipt);
                context.UpdateRange(sequenceNumber);
                context.SaveChanges();

                await mediator.Publish(new ReceiptCreatedEvent()
                {
                    ReceiptNumber = receipt.Number,
                    StorageCode = receipt.StorageCode,
                    UserName = request.Username
                });
            }
            catch (DbUpdateException ex)
            {
                LogHelper.Instance.Error("--- Execute create receipt command at {@time} fail with message {@request}",
                    DateTime.Now.ToString(), ex.InnerException.Message);
                logger.LogError(ex.InnerException.Message);
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
