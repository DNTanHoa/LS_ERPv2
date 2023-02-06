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
    public class UpdateJobOperationCommandHandler
        : IRequestHandler<UpdateJobOperationCommand, CommonCommandResultHasData<JobOperation>>
    {
        private readonly ILogger<UpdateJobOperationCommandHandler> logger;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;

        public UpdateJobOperationCommandHandler(ILogger<UpdateJobOperationCommandHandler> logger,
            IMapper mapper,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.context = context;
        }

        public Task<CommonCommandResultHasData<JobOperation>> Handle(
            UpdateJobOperationCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<JobOperation>();

            logger.LogInformation("{@time} - Execute bulk job operation command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Execute bulk job operation command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var jobOperation = context.JobOperation
                    .FirstOrDefault(x => x.ID == request.ID);

                if (jobOperation != null)
                {
                    mapper.Map(request, jobOperation);
                    jobOperation.SetUpdateAudit(request.UserName);

                    context.JobOperation.Update(jobOperation);
                    context.SaveChanges();

                    result.Success = true;
                    result.SetData(jobOperation);
                }
                else
                {
                    result.Message = "Not found to update";
                }
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Execute update job operation command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Execute update job operation command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
    }
}
