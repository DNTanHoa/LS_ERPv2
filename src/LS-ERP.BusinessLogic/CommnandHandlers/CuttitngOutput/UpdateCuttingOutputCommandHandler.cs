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
    public class UpdateCuttingOutputCommandHandler
        : IRequestHandler<UpdateCuttingOutputCommand, CommonCommandResultHasData<CuttingOutput>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateCuttingOutputCommandHandler> logger;
        private readonly IMapper mapper;

        public UpdateCuttingOutputCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateCuttingOutputCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<CuttingOutput>> Handle(UpdateCuttingOutputCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<CuttingOutput>();
            logger.LogInformation("{@time} - Exceute update cutting output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute update cutting output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var existCuttingOutput = context.CuttingOutput.FirstOrDefault(x => x.ID == request.ID);

                if(existCuttingOutput != null)
                {
                    bool isPrintOld = existCuttingOutput.IsPrint;
                    bool isPrintNew = request.IsPrint;
                    if(isPrintOld == isPrintNew || (isPrintOld == true && isPrintNew == false))
                    {
                        var oldCuttingOutput = existCuttingOutput;
                        if(oldCuttingOutput.IsPrint)
                        {
                            ReturnAllocCuttingOutputPercentPrint(oldCuttingOutput, request.UserName);
                        }   
                        else
                        {
                            ReturnAllocCuttingOutput(oldCuttingOutput, request.UserName);
                        }    
                        
                        mapper.Map(request, existCuttingOutput);
                        var workCenter = context.WorkCenter.Where(x => x.ID == existCuttingOutput.WorkCenterID).FirstOrDefault();
                        existCuttingOutput.WorkCenterName = workCenter.Name;                      
                        var fabricContrast = context.FabricContrast.Where(x => x.ID == existCuttingOutput.FabricContrastID).FirstOrDefault();
                        existCuttingOutput.FabricContrast = fabricContrast.Name;
                        existCuttingOutput.FabricContrastDescription = fabricContrast.Description;
                        existCuttingOutput.IsAllocated = false;
                        existCuttingOutput.ResidualQuantity = 0;
                        existCuttingOutput.SetUpdateAudit(request.UserName);
                        context.CuttingOutput.Update(existCuttingOutput);
                        context.SaveChanges();
                        //Call job alloc Cutting Output
                        var jobId = BackgroundJob.Enqueue<AllocateCuttingOutputJob>(j => j.Execute(existCuttingOutput, request.UserName));
                        result.Success = true;
                        result.SetData(existCuttingOutput);
                    } 
                    else if(isPrintOld == false && isPrintNew == true)
                    {
                        var oldCuttingOutput = existCuttingOutput;
                        ReturnAllocCuttingOutput(oldCuttingOutput, request.UserName);

                        mapper.Map(request, existCuttingOutput);
                        var workCenter = context.WorkCenter.Where(x => x.ID == existCuttingOutput.WorkCenterID).FirstOrDefault();
                        existCuttingOutput.WorkCenterName = workCenter.Name;
                        var fabricContrast = context.FabricContrast.Where(x => x.ID == existCuttingOutput.FabricContrastID).FirstOrDefault();
                        existCuttingOutput.FabricContrast = fabricContrast.Name;
                        existCuttingOutput.FabricContrastDescription = fabricContrast.Description;

                        existCuttingOutput.ResidualQuantity = 0;
                        existCuttingOutput.SetUpdateAudit(request.UserName);
                        context.CuttingOutput.Update(existCuttingOutput);
                        context.SaveChanges();

                        //Call job alloc Cutting Output
                        var jobId = BackgroundJob.Enqueue<AllocateCuttingOutputJob>(j => j.Execute(existCuttingOutput, request.UserName));
                        result.Success = true;
                        result.SetData(existCuttingOutput);
                        
                    }
                }
                else
                {
                    result.Message = "Can't not find to update";
                }
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute update cutting output command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute update cutting output command with request {@request} has error {@error}",
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

            // insert 09092022
            var removeCuttingLots = context.CuttingLot.Where(x => x.CuttingOutputID == cuttingOutput.ID).ToList();
            foreach (var cuttingLot in removeCuttingLots)
            {
                context.CuttingLot.Remove(cuttingLot);
                context.SaveChanges();
            }
            
            //remove cutting Card
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
