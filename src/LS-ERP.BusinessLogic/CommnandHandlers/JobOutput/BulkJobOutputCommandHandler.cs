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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class BulkJobOutputCommandHandler : IRequestHandler<BulkJobOutputCommand,
        CommonCommandResultHasData<IEnumerable<JobOutput>>>
    {
        private readonly ILogger<BulkJobOutputCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public BulkJobOutputCommandHandler(ILogger<BulkJobOutputCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<IEnumerable<JobOutput>>> Handle(
            BulkJobOutputCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<IEnumerable<JobOutput>>();
            logger.LogInformation("{@time} - Exceute bulk job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute bulk job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var jobOutputs = request.Data
                    .Select(x => mapper.Map<JobOutput>(x)).ToList();

                context.JobOuput.AddRange(jobOutputs);
                context.SaveChanges();

                result.Success = true;
                result.Data = jobOutputs;
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute bulk job output command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute bulk job output command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
    }
}
