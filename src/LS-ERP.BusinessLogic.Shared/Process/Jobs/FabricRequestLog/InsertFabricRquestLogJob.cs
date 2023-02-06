using AutoMapper;
using Hangfire;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class InsertFabricRquestLogJob
    {
        private readonly ILogger<InsertFabricRquestLogJob> logger;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;

        public InsertFabricRquestLogJob(ILogger<InsertFabricRquestLogJob> logger,
            IMapper mapper,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.context = context;
        }

        [JobDisplayName("Insert fabric request log")]
        [AutomaticRetry(Attempts = 1)]
        public Task Execute(long fabricRequestID)
        {
            var existFabricRequest = context.FabricRequest
                                            .Include(x => x.Details)
                                            .Include(x => x.Status).FirstOrDefault(x => x.ID == fabricRequestID);

            if (!string.IsNullOrEmpty(existFabricRequest.StatusID) && !existFabricRequest.StatusID.Equals("N"))
            {
                var fabricRequestLog = mapper.Map<FabricRequestLog>(existFabricRequest);

                /// Update handle
                try
                {
                    context.FabricRequestLog.Add(fabricRequestLog);
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    logger.LogError("Insert fabric request log handler has error with message {@message}",
                        ex.InnerException?.Message);
                    LogHelper.Instance.Error("Insert fabric request log event handler has error with message {@message}",
                        ex.InnerException?.Message);
                }
            }


            return Task.CompletedTask;
        }
    }
}
