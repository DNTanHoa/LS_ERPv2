using AutoMapper;
using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
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
    public class BulkJobOperationCommandHandler
        : IRequestHandler<BulkJobOperationCommand, CommonCommandResultHasData<IEnumerable<JobOperation>>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<BulkJobOperationCommandHandler> logger;

        public BulkJobOperationCommandHandler(SqlServerAppDbContext context,
            IMapper mapper,
            ILogger<BulkJobOperationCommandHandler> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }
        public Task<CommonCommandResultHasData<IEnumerable<JobOperation>>> Handle(BulkJobOperationCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<IEnumerable<JobOperation>>();
            logger.LogInformation("{@time} - Execute bulk job operation command with request {@request}",
               DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Execute bulk job operation command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var jobOperations = request.Data
                    .Select(x => mapper.Map<JobOperation>(x)).ToList();
                jobOperations.ForEach(x =>
                {
                    x.SetCreateAudit(request.UserName);
                });

                context.JobOperation.AddRange(jobOperations);
                context.SaveChanges();

                result.Success = true;
                result.SetData(jobOperations);
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Execute bulk job operation command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Execute bulk job operation command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
    }
}
