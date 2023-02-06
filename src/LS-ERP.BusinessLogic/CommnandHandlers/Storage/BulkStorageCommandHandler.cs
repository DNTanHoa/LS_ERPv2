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
    public class BulkStorageCommandHandler
        : IRequestHandler<BulkStorageCommand, CommonCommandResult>
    {
        private readonly ILogger<BulkStorageCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public BulkStorageCommandHandler(ILogger<BulkStorageCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper,
            IMediator mediator)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
            this.mediator = mediator;
        }
        public Task<CommonCommandResult> Handle(BulkStorageCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();
            var storageImportID = 0;

            try
            {
                var storageImport = new StorageImport()
                {
                    CustomerID = request.CustomerID,
                    StorageCode = request.StorageCode,
                    FileName = request.FileName,
                    ProductionMethodCode = request.ProductionMethodCode,
                    Output = request.Output
                };
                storageImport.SetCreateAudit(request.UserName);

                foreach (var storageImportDetailData in request.Data)
                {
                    var storageImportDetail = mapper.Map<StorageImportDetail>(storageImportDetailData);
                    storageImport.Details.Add(storageImportDetail);
                }

                context.StorageImport.Add(storageImport);
                context.SaveChanges();

                result.Success = true;

                storageImportID = storageImport.ID;

                //mediator.Publish(new Events.StorageImportEvent()
                //{
                //    StorageImportID = storageImport.ID
                //});
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }
            finally
            {
                /// Run Hangfire
                if (request.StorageCode == "FB" && request.CustomerID == "DE")
                {
                    if (request.Output)
                    {
                        var jobId = BackgroundJob.Enqueue<CreateIssuedFabricJob>(j => j.Execute(storageImportID));
                    }
                    else
                    {
                        var jobId = BackgroundJob.Enqueue<UpdateOnHandQuantityFabricStorageDetailJob>(j => j.Execute(storageImportID));
                    }
                }
                else
                {
                    var jobId = BackgroundJob.Enqueue<UpdateOnHandQuantityStorageDetailJob>(j => j.Execute(storageImportID));
                }

            }

            return Task.FromResult(result);
        }
    }
}
