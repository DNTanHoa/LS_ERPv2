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
    public class UpdateJobPriceCommandHandler
    : IRequestHandler<UpdateJobPriceCommand, CommonCommandResultHasData<JobPrice>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<UpdateJobPriceCommandHandler> logger;
        private readonly IMapper mapper;

        public UpdateJobPriceCommandHandler(SqlServerAppDbContext context,
            ILogger<UpdateJobPriceCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<JobPrice>> Handle(UpdateJobPriceCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<JobPrice>();
            logger.LogInformation("{@time} - Exceute update job price command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute update job price command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var existJobPrice = context.JobPrice.FirstOrDefault(x => x.ID ==request.ID);

                if (existJobPrice != null)
                {
                    mapper.Map(request, existJobPrice);
                    existJobPrice.SetUpdateAudit(request.UserName);
                    context.JobPrice.Update(existJobPrice);
                    context.SaveChanges();
                    result.Success = true;
                    result.SetData(existJobPrice);
                }
                else
                {
                    result.Message = "Can't not find to update";
                }
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute update job price command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute update job price command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
    }
}
