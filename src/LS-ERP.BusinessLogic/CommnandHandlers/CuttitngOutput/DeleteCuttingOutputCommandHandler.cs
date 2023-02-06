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
    public class DeleteCuttingOutputCommandHandler
        : IRequestHandler<DeleteCuttingOutputCommand, CommonCommandResult>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<DeleteCuttingOutputCommandHandler> logger;

        public DeleteCuttingOutputCommandHandler(SqlServerAppDbContext context,
            ILogger<DeleteCuttingOutputCommandHandler> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public Task<CommonCommandResult> Handle(DeleteCuttingOutputCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            logger.LogInformation("{@time} - Exceute delete cutting output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute delete cutting output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var existCuttingOutput = context.CuttingOutput.FirstOrDefault(x => x.ID == request.ID);

                if (existCuttingOutput != null)
                {
                    if(existCuttingOutput.IsPrint)
                    {
                        ReturnAllocCuttingOutputPercentPrint(existCuttingOutput, request.UserName);
                    }   
                    else
                    {
                        ReturnAllocCuttingOutput(existCuttingOutput, request.UserName);
                    }   
                    context.CuttingOutput.Remove(existCuttingOutput);
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
                logger.LogInformation("{@time} - Exceute delete cutting output command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute delete cutting output command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
        public void ReturnAllocCuttingOutput(CuttingOutput cuttingOutput, string userName)
        {
            var allocTransactions = context.AllocTransaction.Where(x => x.CuttingOutputID == cuttingOutput.ID
                                                                    && x.IsRetured == false
                                                                    && x.Operation == "CUTTING"
                                                                    && string.IsNullOrEmpty(x.Lot)
                                                                ).ToList();
            if (allocTransactions != null)
            {
                foreach (var allocTransaction in allocTransactions)
                {
                    var allocDailyOutput = context.AllocDailyOutput.Where(x => x.ID == allocTransaction.AllocDailyOuputID
                                                                         //&& x.LSStyle == allocTransaction.LSStyle
                                                                         //&& x.Size == allocTransaction.Size
                                                                         //&& x.Set == allocTransaction.Set
                                                                         //&& x.FabricContrastName == allocTransaction.FabricContrastName
                                                                         //&& x.Operation == "CUTTING"
                                                                         ).FirstOrDefault();
                    if (allocDailyOutput != null)
                    {
                        allocDailyOutput.Quantity = allocDailyOutput.Quantity - allocTransaction.AllocQuantity;
                        if(allocDailyOutput.Quantity < (allocDailyOutput.OrderQuantity + allocDailyOutput.Sample + allocDailyOutput.PercentQuantity))
                        {
                            allocDailyOutput.IsFull = false;
                        }
                        allocDailyOutput.SetUpdateAudit(userName);                  
                        context.AllocTransaction.Remove(allocTransaction);
                        context.AllocDailyOutput.Update(allocDailyOutput);
                        context.SaveChanges();
                    }

                }

            }
            var allocTransactions_Lot = context.AllocTransaction.Where(x => x.CuttingOutputID == cuttingOutput.ID
                                                                    && x.IsRetured == false
                                                                    && x.Operation == "CUTTING"
                                                                    && x.Size == cuttingOutput.Size
                                                                    && !string.IsNullOrEmpty(x.Lot)
                                                                ).ToList();
            foreach (var allocTransaction in allocTransactions_Lot)
            {
                context.AllocTransaction.Remove(allocTransaction);
                context.SaveChanges();
            }
            //reverse Lot
            var removeCuttingLots = context.CuttingLot.Where(x => x.CuttingOutputID == cuttingOutput.ID).ToList();
            foreach (var cuttingLot in removeCuttingLots)
            {
                context.CuttingLot.Remove(cuttingLot);
                context.SaveChanges();
            }
            //
            
            var cuttingCards = context.CuttingCard.Where(x => x.CuttingOutputID == cuttingOutput.ID).ToList();
            foreach (var cuttingCard in cuttingCards)
            {
                context.CuttingCard.Remove(cuttingCard);
                context.SaveChanges();
            }
        }
        public void ReturnAllocCuttingOutputPercentPrint(CuttingOutput cuttingOutput, string userName)
        {
            var allocTransactions = context.AllocTransaction.Where(x => x.CuttingOutputID == cuttingOutput.ID
                                                                    && x.IsRetured == false
                                                                    && x.Operation == "CUTTING"
                                                                ).ToList();
            if (allocTransactions != null)
            {
                foreach (var allocTransaction in allocTransactions)
                {
                    var allocDailyOutput = context.AllocDailyOutput.Where(x => x.ID == allocTransaction.AllocDailyOuputID
                                                                         //&& x.LSStyle == allocTransaction.LSStyle
                                                                         //&& x.Size == allocTransaction.Size
                                                                         //&& x.Set == allocTransaction.Set
                                                                         //&& x.FabricContrastName == allocTransaction.FabricContrastName
                                                                         //&& x.Operation == "CUTTING"
                                                                         ).FirstOrDefault();
                    if (allocDailyOutput != null)
                    {
                        allocDailyOutput.PercentPrint = allocDailyOutput.PercentPrint - allocTransaction.AllocQuantity;
                        allocDailyOutput.SetUpdateAudit(userName);
                        context.AllocTransaction.Remove(allocTransaction);
                        context.AllocDailyOutput.Update(allocDailyOutput);
                        context.SaveChanges();
                    }
                }
            }
            //remove cuttingCard
            var cuttingCards = context.CuttingCard.Where(x => x.CuttingOutputID == cuttingOutput.ID).ToList();
            foreach (var cuttingCard in cuttingCards)
            {
                context.CuttingCard.Remove(cuttingCard);
                context.SaveChanges();
            }
        }
    }
}
