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
    public class DeleteDailyTargetDetailCommandHandler
        : IRequestHandler<DeleteDailyTargetDetailCommand, CommonCommandResult>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<DeleteDailyTargetDetailCommandHandler> logger;

        public DeleteDailyTargetDetailCommandHandler(SqlServerAppDbContext context,
            ILogger<DeleteDailyTargetDetailCommandHandler> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public Task<CommonCommandResult> Handle(DeleteDailyTargetDetailCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            logger.LogInformation("{@time} - Exceute delete job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute delete job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var existDailyTargetDetail = context.DailyTargetDetail.FirstOrDefault(x => x.ID == request.ID);

                if (existDailyTargetDetail != null)
                {
                    //reverse Quantity if esxit
                    ReturnAllocDailyOutput(existDailyTargetDetail, request.UserName);
                    context.DailyTargetDetail.Remove(existDailyTargetDetail);

                    context.SaveChanges();
                    result.Success = true;
                }
                else
                {
                    result.Message = "Can't not find to delete";
                }
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute delete job output command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute delete job output command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
        public void ReturnAllocDailyOutput(DailyTargetDetail dailyTargetDetail, string userName)
        {

            var allocTransactions = context.AllocTransaction.Where(x => x.DailyTargetDetailID == dailyTargetDetail.ID
                                                                    && x.IsRetured == false
                                                                ).ToList();
            if (allocTransactions != null)
            {
                foreach (var allocTransaction in allocTransactions)
                {
                    var allocDailyOutput = context.AllocDailyOutput.Where(x => x.LSStyle == allocTransaction.LSStyle
                                                                         && x.Size == allocTransaction.Size
                                                                         ).FirstOrDefault();
                    if (allocDailyOutput != null)
                    {
                        allocDailyOutput.Quantity = allocDailyOutput.Quantity - allocTransaction.AllocQuantity;
                        allocDailyOutput.IsFull = false;
                        allocDailyOutput.SetUpdateAudit(userName);
                        allocTransaction.IsRetured = true;
                        allocTransaction.SetUpdateAudit(userName);
                        context.SaveChanges();
                    }

                }

            }

        }
    }
}
