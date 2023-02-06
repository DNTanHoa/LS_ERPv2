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
    public class CreateJobPriceCommandHandler
    : IRequestHandler<CreateJobPriceCommand, CommonCommandResultHasData<JobPrice>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateJobPriceCommandHandler> logger;
        private readonly IMapper mapper;

        public CreateJobPriceCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateJobPriceCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<JobPrice>> Handle(CreateJobPriceCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<JobPrice>();
            logger.LogInformation("{@time} - Exceute create job price command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute create job price command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            var existPartPrice = context.JobPrice.FirstOrDefault(x => x.CompanyID == request.CompanyID
                                                               && x.Operation == request.Operation
                                                               && x.Price == request.Price);

            if (existPartPrice == null)
            {
                var jobPrice = mapper.Map<JobPrice>(request);
                try
                {
                    jobPrice.SetCreateAudit(request.UserName);
                    context.JobPrice.Add(jobPrice);
                    context.SaveChanges();
                    result.Success = true;
                    result.SetData(jobPrice);
                }
                catch (DbUpdateException ex)
                {
                    result.Message = ex.Message;
                    logger.LogInformation("{@time} - Exceute create job price command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                    LogHelper.Instance.Information("{@time} - Exceute create job price command with request {@request} has error {@error}",
                        DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                }
            }

            return Task.FromResult(result);
        }

    }
}
