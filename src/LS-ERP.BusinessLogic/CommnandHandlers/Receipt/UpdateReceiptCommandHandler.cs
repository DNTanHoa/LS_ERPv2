using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Extensions;
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
    public class UpdateReceiptCommandHandler
        : IRequestHandler<UpdateReceiptCommand, CommonCommandResult<Receipt>>
    {
        private readonly ILogger<UpdateReceiptCommandHandler> logger;
        private readonly IMaterialTransactionRepository materialTransactionRepository;
        private readonly IReceiptRepository receiptRepository;
        private readonly IStorageDetailRepository storageDetailRepository;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;

        public UpdateReceiptCommandHandler(ILogger<UpdateReceiptCommandHandler> logger,
            IMaterialTransactionRepository materialTransactionRepository,
            IReceiptRepository receiptRepository,
            IStorageDetailRepository storageDetailRepository,
            IMapper mapper,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.materialTransactionRepository = materialTransactionRepository;
            this.receiptRepository = receiptRepository;
            this.storageDetailRepository = storageDetailRepository;
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<CommonCommandResult<Receipt>> Handle(
            UpdateReceiptCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("--- Execute update receipt command at {@time} with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(request));
            LogHelper.Instance.Information("--- Execute update receipt command at {@time} with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(request));
            
            var result = new CommonCommandResult<Receipt>();

            if (receiptRepository.IsExist(request.Number))
            {
                result.Message = $"Can't find receipt with number {request.Number} to update";
                return result;
            }

            var receipt = receiptRepository.GetReceipt(request.Number);
            var purchaseOrderGroupLines = context.PurchaseOrderGroupLine
                .Where(x => receipt.ReceiptGroupLines.Select(x => x.PurchaseOrderGroupLineID).Contains(x.ID))
                .ToList();

            mapper.Map(request, receipt);
            receipt.SetUpdateAudit(request.Username);

            ///Reverse old material transaction
            var oldMaterialTransactions = materialTransactionRepository
                .GetMaterialTransactionsOfReceipt(request.Number);
            oldMaterialTransactions.ToList().ReverseAll();

            /// Create new material transaction
            var transactions = MaterialTransactionProcess.CreateTransactions(receipt, request.GetUser());

            /// Caculate and update storage detail
            var storageDetails = storageDetailRepository.GetStorageDetails(receipt.StorageCode,
                receipt.PurchaseOrder?.Number);
            StorageDetailProcess.UpdateStorageFromReceipt(storageDetails.ToList(), transactions, request.StorageCode,
                request.GetUser(),
                out List<StorageDetail> newStorageDetail);
            
            UpdatePurchaseOrderGroupLineQuantityProcess.CalculateReceiptQuantity(purchaseOrderGroupLines.ToList(),
                receipt.ReceiptGroupLines.ToList(),
                out string errorMessage);

            try
            {
                context.MaterialTransaction.UpdateRange(oldMaterialTransactions);
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                LogHelper.Instance.Error("--- Execute update receipt command at {@time} fail with message {@request}",
                    DateTime.Now.ToString(), ex.InnerException.Message);
                logger.LogError(ex.InnerException.Message);
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
