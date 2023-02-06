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
    public class CreateOperationCommandHandler
        : IRequestHandler<CreateOperationCommand, CommonCommandResultHasData<Operation>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateOperationCommandHandler> logger;
        private readonly IMapper mapper;

        public CreateOperationCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateOperationCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<Operation>> Handle(CreateOperationCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<Operation>();
            logger.LogInformation("{@time} - Exceute create operation command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute operation command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            var Operation = new Operation();
            Operation = mapper.Map<Operation>(request);
            try
            {
                Operation.SetCreateAudit(request.UserName);
                context.Operation.Add(Operation);
                context.SaveChanges();                 
                result.Success = true;
                result.SetData(Operation);
               
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute create operation command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute create operation command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }
            return Task.FromResult(result);
        }
       
    }
}
