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
    public class UpdateOperationCommandHandler
        : IRequestHandler<UpdateOperationCommand, CommonCommandResultHasData<Operation>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateOperationCommandHandler> logger;
        private readonly IMapper mapper;

        public UpdateOperationCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateOperationCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<Operation>> Handle(UpdateOperationCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<Operation>();
            logger.LogInformation("{@time} - Exceute update Operation command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute update Operation command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var existOperation = context.Operation.FirstOrDefault(x => x.ID == request.ID);

                if(existOperation != null)
                {
                    
                    mapper.Map(request, existOperation);
                        
                    existOperation.SetUpdateAudit(request.UserName);
                    context.Operation.Update(existOperation);
                    context.SaveChanges();
                    result.Success = true;
                    result.SetData(existOperation);
                        
                    
                }
                else
                {
                    result.Message = "Can't not find to update";                
                }
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute update Operation command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute update Operation command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
        
    }
}
