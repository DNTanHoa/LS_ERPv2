using AutoMapper;
using Common.Model;
using Hangfire;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Shared.Process.Jobs;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
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
    public class BulkStorageBinEntryCommandHandler
        : IRequestHandler<BulkStorageBinEntryCommand, CommonCommandResult>
    {
        private readonly ILogger<BulkStorageBinEntryCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public BulkStorageBinEntryCommandHandler(ILogger<BulkStorageBinEntryCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper,
            IMediator mediator)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
            this.mediator = mediator;
        }

        public Task<CommonCommandResult> Handle(BulkStorageBinEntryCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();
            var storageBinEntries = new List<StorageBinEntry>();

            try
            {
                foreach (var data in request.Data)
                {
                    var storageBinEntry = new StorageBinEntry();
                    storageBinEntry.CustomerID = request.CustomerID;
                    storageBinEntry.CustomerPurchaseOrderNumber = data.PurchaseOrderNumber;
                    storageBinEntry.StorageCode = request.StorageCode;
                    storageBinEntry.Factory = request.Factory;
                    storageBinEntry.CustomerStyle = data.CustomerStyle;
                    storageBinEntry.LSStyle=data.LSStyle;
                    storageBinEntry.GarmentColorCode = data.GarmentColorCode;
                    storageBinEntry.GarmentColorName = data.GarmentColorName;
                    storageBinEntry.Season= data.Season;
                    storageBinEntry.BinCode=data.BinCode;
                    storageBinEntry.SetCreateAudit(request.UserName);
                    storageBinEntry.EntryDate = DateTime.Now;

                    storageBinEntries.Add(storageBinEntry);
                }

                context.StorageBinEntry.AddRange(storageBinEntries);
                context.SaveChanges();

                result.Success = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }
            finally
            {
                /// Run Hangfire
                    var jobId = BackgroundJob.Enqueue<UpdateBinCodeStorageDetailJob>(j => j.Execute(storageBinEntries));
            }

            return Task.FromResult(result);
        }
    }
}
