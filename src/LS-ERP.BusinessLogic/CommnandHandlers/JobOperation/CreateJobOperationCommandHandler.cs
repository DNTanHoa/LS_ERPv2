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
    public class CreateJobOperationCommandHandler
        : IRequestHandler<CreateJobOperationCommand, CommonCommandResultHasData<JobOperation>>
    {
        private readonly ILogger<CreateJobOperationCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public CreateJobOperationCommandHandler(ILogger<CreateJobOperationCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<JobOperation>> Handle(
            CreateJobOperationCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<JobOperation>();
            
            logger.LogInformation("{@time} - Exceute bulk job operation command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute bulk job operation command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var jobOperation = mapper.Map<JobOperation>(request);
                jobOperation.SetCreateAudit(request.UserName);
                context.JobOperation.Add(jobOperation);

                context.SaveChanges();

                result.Success = true;
                result.SetData(jobOperation);
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute bulk job operation command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute bulk job operation command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
    }
}
