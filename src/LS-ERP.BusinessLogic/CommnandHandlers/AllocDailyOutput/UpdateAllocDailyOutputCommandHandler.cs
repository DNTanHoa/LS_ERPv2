using AutoMapper;
using Common.Model;
using Hangfire;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Shared.Process.Jobs;
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
    public class UpdateAllocDailyOutputCommandHandler
        : IRequestHandler<UpdateAllocDailyOutputCommand, CommonCommandResultHasData<AllocDailyOutput>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<UpdateAllocDailyOutputCommandHandler> logger;
        private readonly IMapper mapper;

        public UpdateAllocDailyOutputCommandHandler(SqlServerAppDbContext context,
            ILogger<UpdateAllocDailyOutputCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<AllocDailyOutput>> Handle(UpdateAllocDailyOutputCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<AllocDailyOutput>();
            logger.LogInformation("{@time} - Exceute update alloc daily output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute update alloc daily output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var existAllocDailyOutput = context.AllocDailyOutput.FirstOrDefault(x => x.ID == request.ID);

                if(existAllocDailyOutput != null)
                {
                    mapper.Map(request, existAllocDailyOutput);                       
                    existAllocDailyOutput.SetUpdateAudit(request.UserName);
                    context.AllocDailyOutput.Update(existAllocDailyOutput);
                    context.SaveChanges();
               
                    result.Success = true;
                    result.SetData(existAllocDailyOutput);                        
                    
                }
                else
                {
                    result.Message = "Can't not find to update";
                }
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute update alloc daily output command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute update alloc daily output command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
        
    }
}
