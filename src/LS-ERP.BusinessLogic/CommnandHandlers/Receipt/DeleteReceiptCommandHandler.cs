using AutoMapper;
using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ultils.Helpers;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class DeleteReceiptCommandHandler 
        : IRequestHandler<DeleteReceiptCommand, CommonCommandResult>
    {
        private readonly ILogger<CreateReceiptCommandHandler> logger;
        private readonly IReceiptRepository receiptRepository;
        private readonly IEntitySequenceNumberRepository entitySequenceNumberRepository;
        private readonly IStorageDetailRepository storageDetailRepository;
        private readonly IPurchaseOrderGroupLineRepository purchaseOrderGroupLineRepository;
        private readonly IFabricPurchaseOrderRepository fabricPurchaseOrderRepository;
        private IMapper mapper;
        private readonly SqlServerAppDbContext context;
        private readonly IConfiguration configuration;

        public DeleteReceiptCommandHandler(ILogger<CreateReceiptCommandHandler> logger,
            IReceiptRepository receiptRepository,
            IEntitySequenceNumberRepository entitySequenceNumberRepository,
            IStorageDetailRepository storageDetailRepository,
            IPurchaseOrderGroupLineRepository purchaseOrderGroupLineRepository,
            IFabricPurchaseOrderRepository fabricPurchaseOrderRepository,
            IMapper mapper,
            SqlServerAppDbContext context,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.receiptRepository = receiptRepository;
            this.entitySequenceNumberRepository = entitySequenceNumberRepository;
            this.storageDetailRepository = storageDetailRepository;
            this.purchaseOrderGroupLineRepository = purchaseOrderGroupLineRepository;
            this.fabricPurchaseOrderRepository = fabricPurchaseOrderRepository;
            this.context = context;
            this.configuration = configuration;
            this.receiptRepository = receiptRepository;
            this.entitySequenceNumberRepository = entitySequenceNumberRepository;
            this.storageDetailRepository = storageDetailRepository;
            this.purchaseOrderGroupLineRepository = purchaseOrderGroupLineRepository;
            this.fabricPurchaseOrderRepository = fabricPurchaseOrderRepository;
            this.mapper = mapper;
            this.context = context;
            this.logger = logger;
        }
        public Task<CommonCommandResult> Handle(DeleteReceiptCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            try
            {
                // Lấy thông tin purchase order để cập nhật thông tin
                var connectionString = configuration.GetSection("ConnectionString")
                .GetSection("DbConnection").Value ?? string.Empty;
                var sqlParameter = new SqlParameter[]
                {
                    new SqlParameter("@ReceiptNumber", request.ReceiptNumber),
                };
                var resultSQL = SqlHelper.ExecuteNonQuery(connectionString,
                    "sp_DeleteReceipt", sqlParameter);

                result.Success = true;
            }
            catch(Exception ex)
            {
                result.Message = ex.Message;
            }

            return Task.FromResult(result);
        }
    }
}
