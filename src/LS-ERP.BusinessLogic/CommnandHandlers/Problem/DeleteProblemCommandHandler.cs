using Common.Model;
using LS_ERP.BusinessLogic.Commands;
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
    internal class DeleteProblemCommandHandler : 
        IRequestHandler<DeleteProblemCommand, CommonCommandResult>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<DeleteProblemCommandHandler> logger;

        public DeleteProblemCommandHandler(SqlServerAppDbContext context,
            ILogger<DeleteProblemCommandHandler> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public Task<CommonCommandResult> Handle(DeleteProblemCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            logger.LogInformation("{@time} - Exceute delete problem command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute delete problem command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var existProblem = context.Problem.FirstOrDefault(x => x.ID == request.ID);

                if (existProblem != null)
                {
                    context.Problem.Remove(existProblem);
                    context.SaveChanges();

                    result.Success = true;
                }
                else
                {
                    result.Message = "Can't not find to delete";
                }
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute delete problem command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute delete problem command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
    }
}
