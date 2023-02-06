using AutoMapper;
using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
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
    public class CreateProblemCommandHandler : 
        IRequestHandler<CreateProblemCommand, CommonCommandResultHasData<Problem>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateProblemCommandHandler> logger;
        private readonly IMapper mapper;

        public CreateProblemCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateProblemCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<Problem>> Handle(CreateProblemCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<Problem>();
            logger.LogInformation("{@time} - Exceute create problem command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute create problem command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            var problem = mapper.Map<Problem>(request);
            // set OutputAt
            problem.CreatedAt = DateTime.Now;
            try
            {
                problem.SetCreateAudit(request.UserName);
                context.Problem.Add(problem);
                context.SaveChanges();

                result.Success = true;
                result.SetData(problem);
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute create problem command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute create problem command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
    }
}
