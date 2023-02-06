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
    public class UpdateFabricContrastCommandHandler
        : IRequestHandler<UpdateFabricContrastCommand, CommonCommandResultHasData<FabricContrast>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<UpdateFabricContrastCommandHandler> logger;
        private readonly IMapper mapper;

        public UpdateFabricContrastCommandHandler(SqlServerAppDbContext context,
            ILogger<UpdateFabricContrastCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<FabricContrast>> Handle(UpdateFabricContrastCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<FabricContrast>();
            logger.LogInformation("{@time} - Exceute update FabricContrast command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute update FabricContrast command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var existFabricContrast = context.FabricContrast.FirstOrDefault(x => x.ID == request.ID);

                if(existFabricContrast != null)
                {
                    
                    mapper.Map(request, existFabricContrast);
                    existFabricContrast.SetUpdateAudit(request.UserName);
                    context.FabricContrast.Update(existFabricContrast);
                    context.SaveChanges();
                    result.Success = true;
                    result.SetData(existFabricContrast);
                }
                else
                {
                    result.Message = "Can't not find to update";                
                }
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute update FabricContrast command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute update FabricContrast command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
        
    }
}
