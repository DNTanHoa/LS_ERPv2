using AutoMapper;
using Hangfire;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ultils.Helpers;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class AllocateCuttingOutputJob
    {
        private readonly ILogger<AllocateCuttingOutputJob> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        

        public AllocateCuttingOutputJob(ILogger<AllocateCuttingOutputJob> logger,
            SqlServerAppDbContext context, IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
            
        }

        [JobDisplayName("Allocate Cutting Output when create,update CuttingOutput")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute(CuttingOutput cuttingOutput,string userName)
        {
            
            try
            {
                //For not % print
             
                if(!cuttingOutput.IsPrint)
                {  
                    //Reverse Quantity if exist
                    ReturnAllocCuttingOutput(cuttingOutput, userName);
                    //// Auto alloc
                  
                    ///// alloc for HADDAD
                    ///
                    if (cuttingOutput.CustomerID.Equals("HA")) 
                    {
                        AllocCuttingOutput_For_Haddad(cuttingOutput,userName);                          
                    }
                  
                    /// alloc for GARAN
                    ///
                    else if (cuttingOutput.CustomerID.Equals("GA"))
                    {
                        AllocCuttingOutput_For_Garan(cuttingOutput, userName);
                    }
                    else //alloc other customer
                    {
                        AllocCuttingOutput_For_Decathlon(cuttingOutput, userName);
                    }
                }
                //For percent print
                if(cuttingOutput.IsPrint)
                {
                    AllocCuttingOutputPercentPrint(cuttingOutput, userName);
                    CreateCuttingCardPerCentPrint(cuttingOutput, userName);
                }    
                
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("Allocate cutting output event handler has error with message {@message}",
                    ex.InnerException?.Message);
                LogHelper.Instance.Error("Allocate cutting output event handler has error with message {@message}",
                    ex.InnerException?.Message);
            }

            return Task.CompletedTask;  
        }
        public Task Execute(List<CuttingOutput> cuttingOutputs, string userName)
        {

            try
            {
                foreach (var cuttingOutput in cuttingOutputs)
                {


                    //For not % print
                 
                    if (!cuttingOutput.IsPrint)
                    {
                        //Reverse Quantity if exist
                        ReturnAllocCuttingOutput(cuttingOutput, userName);
                        //// Auto alloc
                     
                        ///// alloc for HADDAD                      
                        if (cuttingOutput.CustomerID.Equals("HA"))
                        {
                            AllocCuttingOutput_For_Haddad(cuttingOutput, userName);                             
                        }
                      
                        /// alloc for GARAN                  
                        else if (cuttingOutput.CustomerID.Equals("GA"))
                        {
                            AllocCuttingOutput_For_Garan(cuttingOutput,userName);                            
                        }
                        else //alloc other customer
                        {
                            AllocCuttingOutput_For_Decathlon(cuttingOutput, userName);                            
                        }
                    }
                    //For percent print
                    if (cuttingOutput.IsPrint)
                    {
                        AllocCuttingOutputPercentPrint(cuttingOutput, userName);
                        CreateCuttingCardPerCentPrint(cuttingOutput, userName);
                    }
                }
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("Allocate list cutting output event handler has error with message {@message}",
                    ex.InnerException?.Message);
                LogHelper.Instance.Error("Allocate list cutting output event handler has error with message {@message}",
                    ex.InnerException?.Message);
            }

            return Task.CompletedTask;
        }
        public void AllocCuttingOutput_For_Haddad(CuttingOutput cuttingOutput, string userName)
        {
            if (cuttingOutput.Quantity > 0)
            {
                decimal allocQuantity = (decimal)cuttingOutput.Quantity;
                //if priority lsstyle exist --> alloc to priority lsstyle
                if (!string.IsNullOrEmpty(cuttingOutput.PriorityLSStyle))
                {
                    allocQuantity = AllocCuttingOutputToLSStyle_NotSample(cuttingOutput, allocQuantity, cuttingOutput.PriorityLSStyle, userName);
                }
                // continue alloc to merge lsstyle if allocQuantity > 0
                if (allocQuantity > 0)
                {
                    if (!string.IsNullOrEmpty(cuttingOutput.MergeLSStyle))
                    {
                        var dailayTarget = context.DailyTarget.Where(x => x.MergeLSStyle == cuttingOutput.MergeLSStyle
                                                                     //&& x.FromDate.Date >= cuttingOutput.FromDate.Date
                                                                     //&& x.ToDate.Date <= cuttingOutput.ToDate.Date
                                                                     && x.MergeBlockLSStyle == cuttingOutput.MergeBlockLSStyle
                                                                     && x.IsCanceled == false
                                                                     && x.Operation == "CUTTING"
                                                                    ).FirstOrDefault();
                        if (dailayTarget != null)
                        {
                            if (!string.IsNullOrEmpty(dailayTarget.MergeLSStyle))
                            {
                                var listLSStyle = dailayTarget.MergeLSStyle.Split("+");
                                foreach (var item in listLSStyle)
                                {
                                    var str = dailayTarget.MergeBlockLSStyle;
                                    var prefix = str.Substring(0, str.LastIndexOf("-"));
                                    var LSStyle = prefix + "-" + item.Trim();
                                    if (allocQuantity > 0)
                                    {
                                        allocQuantity = AllocCuttingOutputToLSStyle_NotSample(cuttingOutput, allocQuantity, LSStyle, userName);
                                    }
                                }
                            }
                        }
                    }
                }
                // PSDD first
                if (allocQuantity > 0)
                {
                    var lsStyles = (from d in context.DailyTarget.Where(x => x.MergeBlockLSStyle == cuttingOutput.MergeBlockLSStyle
                                                                     && x.IsCanceled == false
                                                                     && x.Operation == "CUTTING"
                                                                    )

                                    from a in context.AllocDailyOutput.Where(x => x.Operation == "CUTTING"
                                                                                && d.ID == x.TargetID
                                                                                && x.FabricContrastName == cuttingOutput.FabricContrast)
                                    select new HALSStyle
                                    {
                                        LSStyle = a.LSStyle,
                                        Size = a.Size,
                                        PSDD = d.PSDD,
                                        EstimatedPort = d.EstimatedPort
                                    }
                                    ).ToList();     
                    foreach(var item in lsStyles)
                    {
                        var priority = context.AllocPriority.Where(x => x.PriorityName == item.EstimatedPort).FirstOrDefault();
                        item.Index = priority == null ? 50 : priority.Index;
                    }
                    lsStyles = lsStyles.OrderBy(o => o.PSDD).ThenBy(o => o.Index).ToList();
                 
                    foreach (var lsStyle in lsStyles)
                    {
                        if (allocQuantity > 0)
                        {
                            allocQuantity = AllocCuttingOutputToLSStyle_NotSample(cuttingOutput, allocQuantity, lsStyle.LSStyle, userName);
                        }
                    }
                    if(allocQuantity>0)
                    {
                        var currentLSStyle = "";
                        var curentQuantity = allocQuantity;
                        foreach (var lsStyle in lsStyles)
                        {
                            if (allocQuantity > 0)
                            {
                                allocQuantity = AllocCuttingOutputToLSStyle_Sample(cuttingOutput, allocQuantity, lsStyle.LSStyle, userName);
                                if (allocQuantity != curentQuantity)
                                {
                                    curentQuantity = allocQuantity;
                                    currentLSStyle = lsStyle.LSStyle;
                                }
                            }
                        }
                        // Set over orderQuantity for last lsstyle                        
                        if (allocQuantity > 0)
                        {
                            var currentLSStyles = lsStyles.Where(x => x.LSStyle == currentLSStyle && x.Size == cuttingOutput.Size).FirstOrDefault();
                            if (currentLSStyles != null)
                            {
                                var lastLSStyle = currentLSStyles.LSStyle;
                                AllocCuttingOutputToLSStyle_Over(cuttingOutput, allocQuantity, lastLSStyle, userName);
                            }
                            else
                            {
                                var lastLSStyles = lsStyles.Where(x => x.Size == cuttingOutput.Size).LastOrDefault();
                                if (lastLSStyles != null)
                                {
                                    var lastLSStyle = lastLSStyles.LSStyle;
                                    AllocCuttingOutputToLSStyle_Over(cuttingOutput, allocQuantity, lastLSStyle, userName);
                                }
                            }
                        }
                    } 
                }
                if (allocQuantity == 0)
                {
                    cuttingOutput.IsAllocated = true;
                    cuttingOutput.SetUpdateAudit(userName);
                    context.CuttingOutput.Update(cuttingOutput);
                    context.SaveChanges();
                }
                else
                {
                    cuttingOutput.IsAllocated = true;
                    cuttingOutput.ResidualQuantity = allocQuantity;
                    cuttingOutput.SetUpdateAudit(userName);
                    context.CuttingOutput.Update(cuttingOutput);
                    context.SaveChanges();
                }
                // Create CuttingCard
                CreateCuttingCard(cuttingOutput, userName);
                
            }
        }
        public void AllocCuttingOutput_For_Garan(CuttingOutput cuttingOutput,string userName)
        {
            if (cuttingOutput.Quantity > 0)
            {
                decimal allocQuantity = (decimal)cuttingOutput.Quantity;
                //if priority lsstyle exist --> alloc to priority lsstyle
                if (!string.IsNullOrEmpty(cuttingOutput.PriorityLSStyle))
                {
                    allocQuantity = AllocCuttingOutputToLSStyle(cuttingOutput, allocQuantity, cuttingOutput.PriorityLSStyle, userName);
                }
                // continue alloc to merge lsstyle if allocQuantity > 0
                if (allocQuantity > 0)
                {
                    if (!string.IsNullOrEmpty(cuttingOutput.MergeLSStyle))
                    {
                        var dailayTarget = context.DailyTarget.Where(x => x.MergeLSStyle == cuttingOutput.MergeLSStyle
                                                                     //&& x.FromDate.Date >= cuttingOutput.FromDate.Date
                                                                     //&& x.ToDate.Date <= cuttingOutput.ToDate.Date
                                                                     && x.MergeBlockLSStyle == cuttingOutput.MergeBlockLSStyle
                                                                     && x.IsCanceled == false
                                                                     && x.Operation == "CUTTING"
                                                                    ).FirstOrDefault();
                        if (dailayTarget != null)
                        {
                            if (!string.IsNullOrEmpty(dailayTarget.MergeLSStyle))
                            {
                                var listLSStyle = dailayTarget.MergeLSStyle.Split("+");
                                foreach (var item in listLSStyle)
                                {
                                    var str = dailayTarget.MergeBlockLSStyle;
                                    var prefix = str.Substring(0, str.LastIndexOf("-"));
                                    var LSStyle = prefix + "-" + item.Trim();
                                    if (allocQuantity > 0)
                                    {
                                        allocQuantity = AllocCuttingOutputToLSStyle(cuttingOutput, allocQuantity, LSStyle, userName);
                                    }
                                }
                            }
                        }
                    }
                }
                // continue alloc to merge block lsstyle if allocQuantiy > 0 // OrderType: priority in Allocpriority    
                if (allocQuantity > 0)
                {
                    //var ls =  (from d in context.DailyTarget.Where(x => x.MergeBlockLSStyle == cuttingOutput.MergeBlockLSStyle
                    //                                                    && x.IsCanceled == false
                    //                                                    && x.Operation == "CUTTING" )
                    //            from a in context.AllocDailyOutput.Where(x => x.Operation == "CUTTING"
                    //                                                && x.TargetID == d.ID)
                    //            from ap in context.AllocPriority.Where(x=>x.CustomerID == "GA"
                    //                                                && x.PriorityName.Contains(d.OrderTypeName.Replace(" ","")) )
                    //            select new GALSStyle { 
                    //                LSStyle = d.LSStyle,
                    //                Size = a.Size,
                    //                OrderTypeName= ap.PriorityName,
                    //                Index = ap.Index
                    //            }).ToList();
                    var ls = (from d in context.DailyTarget.Where(x => x.MergeBlockLSStyle == cuttingOutput.MergeBlockLSStyle
                                                                        && x.IsCanceled == false
                                                                        && x.Operation == "CUTTING")
                              from a in context.AllocDailyOutput.Where(x => x.Operation == "CUTTING"
                                                                  && x.TargetID == d.ID)                             
                              select new GALSStyle
                              {
                                  LSStyle = d.LSStyle,
                                  OrderTypeName = d.OrderTypeName,
                                  Size = a.Size                                  
                              }).ToList();
                    foreach(var l in ls)
                    {
                        var ap = context.AllocPriority.Where(a => a.CustomerID =="GA"
                                                            && a.PriorityName.Contains(l.OrderTypeName.Replace(" ", ""))
                                                            ).FirstOrDefault();
                        if(ap != null)
                        {
                            l.Index = ap.Index;
                        }    
                        else
                        {
                            l.Index = 50;
                        }    
                    }    
                    var lsStyles = ls.OrderBy(o => o.Index).ToList();
                    
                    var currentLSStyle = "";
                    var curentQuantity = allocQuantity;
                    foreach (var lsStyle in lsStyles)
                    {
                        if (allocQuantity > 0)
                        {
                            allocQuantity = AllocCuttingOutputToLSStyle(cuttingOutput, allocQuantity, lsStyle.LSStyle, userName);
                            if (allocQuantity != curentQuantity)
                            {
                                curentQuantity = allocQuantity;
                                currentLSStyle = lsStyle.LSStyle;
                            }
                        }
                    }
                    if (lsStyles.Count > 0 && allocQuantity > 0)
                    {
                        // Set over orderQuantity for last lsstyle
                        if (cuttingOutput.IsAllSize)
                        {
                            var lsstyle = lsStyles.Last().LSStyle;
                            AllocCuttingOutputToLSStyle_Over(cuttingOutput, allocQuantity, lsstyle, userName);
                        }
                        else
                        {
                            var currentLSStyles = lsStyles.Where(x => x.LSStyle == currentLSStyle && x.Size == cuttingOutput.Size).FirstOrDefault();
                            if (currentLSStyles != null)
                            {
                                var lsstyle = currentLSStyles.LSStyle;
                                AllocCuttingOutputToLSStyle_Over(cuttingOutput, allocQuantity, lsstyle, userName);
                            }
                            else
                            {
                                var lastLSStyle = lsStyles.Where(x => x.Size == cuttingOutput.Size).LastOrDefault();
                                if (lastLSStyle != null)
                                {
                                    var lsstyle = lastLSStyle.LSStyle;
                                    AllocCuttingOutputToLSStyle_Over(cuttingOutput, allocQuantity, lsstyle, userName);
                                }
                            }
                        }
                    }
                }
               
                if (allocQuantity == 0)
                {
                    cuttingOutput.IsAllocated = true;
                    cuttingOutput.SetUpdateAudit(userName);
                    context.CuttingOutput.Update(cuttingOutput);
                    context.SaveChanges();
                }
                else
                {
                    cuttingOutput.IsAllocated = true;
                    cuttingOutput.ResidualQuantity = allocQuantity;
                    cuttingOutput.SetUpdateAudit(userName);
                    context.CuttingOutput.Update(cuttingOutput);
                    context.SaveChanges();
                }
                // Create CuttingCard
                CreateCuttingCard(cuttingOutput, userName);
            }
        }
        public void AllocCuttingOutput_For_Decathlon(CuttingOutput cuttingOutput,string userName)
        {
            if (cuttingOutput.Quantity > 0)
            {
                decimal allocQuantity = (decimal)cuttingOutput.Quantity;
                //if priority lsstyle exist --> alloc to priority lsstyle
                var currentPriorityLSStyle = "";
                if (!string.IsNullOrEmpty(cuttingOutput.PriorityLSStyle))
                {
                    var currentQty = allocQuantity;
                    allocQuantity = AllocCuttingOutputToLSStyle(cuttingOutput, allocQuantity, cuttingOutput.PriorityLSStyle, userName);
                    if(currentQty != allocQuantity)
                    {
                        currentPriorityLSStyle = cuttingOutput.PriorityLSStyle;
                    }  
                }
                // continue alloc to merge lsstyle if allocQuantity > 0
                var currentLSStyleForMerge = "";
                if (allocQuantity > 0)
                {
                    if (!string.IsNullOrEmpty(cuttingOutput.MergeLSStyle))
                    {
                        var dailayTarget = context.DailyTarget.Where(x => x.MergeLSStyle == cuttingOutput.MergeLSStyle
                                                                     // && x.FromDate.Date >= cuttingOutput.FromDate.Date
                                                                     // && x.ToDate.Date <= cuttingOutput.ToDate.Date
                                                                     && x.MergeBlockLSStyle == cuttingOutput.MergeBlockLSStyle
                                                                     && x.IsCanceled == false
                                                                     && x.IsDeleted == false
                                                                     && x.Operation == "CUTTING"
                                                                    ).FirstOrDefault();
                        if (dailayTarget != null)
                        {
                            if (!string.IsNullOrEmpty(dailayTarget.MergeLSStyle))
                            {
                                var listLSStyle = dailayTarget.MergeLSStyle.Split("+");
                                foreach (var item in listLSStyle)
                                {
                                    var str = dailayTarget.MergeBlockLSStyle;
                                    var prefix = str.Substring(0, str.LastIndexOf("-"));
                                    var LSStyle = prefix + "-" + item.Trim();
                                    var currentQty = allocQuantity;
                                    if (allocQuantity > 0)
                                    {
                                        allocQuantity = AllocCuttingOutputToLSStyle(cuttingOutput, allocQuantity, LSStyle, userName);
                                        if(currentQty != allocQuantity)
                                        {
                                            currentQty = allocQuantity;
                                            currentLSStyleForMerge = LSStyle;
                                        }     
                                    }
                                }
                            }
                        }
                    }
                }
                // continue alloc to merge block lsstyle if allocQuantiy > 0
                if (allocQuantity > 0)
                {
                    var dailayTargetIDs = context.DailyTarget.Where(x => x.MergeBlockLSStyle == cuttingOutput.MergeBlockLSStyle
                                                                     && x.IsCanceled == false
                                                                     && x.IsDeleted == false
                                                                     && x.Operation == "CUTTING"
                                                                    ).OrderBy(x => x.PSDD).Select(x => x.ID).ToList();

                    var lsStyles = context.AllocDailyOutput.Where(x => x.Operation == "CUTTING"
                                                                    && dailayTargetIDs.Contains(x.TargetID))
                                                                    .Select(x => new DELSStyle
                                                                    {
                                                                        PSDD = x.PSDD,
                                                                        LSStyle = x.LSStyle,
                                                                        Size = x.Size,
                                                                        Index = 0
                                                                    })
                                                                    .Distinct()
                                                                    .OrderBy(x => x.PSDD)
                                                                    .ThenBy(x => x.Index)
                                                                    .ToList();
                    if (lsStyles.Count > 0)
                    {
                        foreach (var item in lsStyles)
                        {
                            int idx = 0;
                            int.TryParse(item.LSStyle.Substring(item.LSStyle.LastIndexOf("-") + 2, item.LSStyle.Length - (item.LSStyle.LastIndexOf("-") + 2)), out idx);
                            item.Index = idx;
                        }
                        lsStyles = lsStyles.OrderBy(x => x.PSDD).ThenBy(x => x.Index).ToList();
                    }
                    var currentLSStyle = "";
                    var curentQuantity = allocQuantity;
                    foreach (var lsStyle in lsStyles)
                    {
                        if (allocQuantity > 0)
                        {
                            allocQuantity = AllocCuttingOutputToLSStyle(cuttingOutput, allocQuantity, lsStyle.LSStyle, userName);
                            if (allocQuantity != curentQuantity)
                            {
                                curentQuantity = allocQuantity;
                                currentLSStyle = lsStyle.LSStyle;
                            }
                        }
                    }
                    if (lsStyles.Count > 0 && allocQuantity > 0)
                    {
                        // Set over orderQuantity for last lsstyle
                        if (cuttingOutput.IsAllSize)
                        {
                            var lsstyle = lsStyles.Last().LSStyle;
                            AllocCuttingOutputToLSStyle_Over(cuttingOutput, allocQuantity, lsstyle, userName);
                        }
                        else
                        {
                            var currentLSStyles = lsStyles.Where(x => x.LSStyle == currentLSStyle && x.Size == cuttingOutput.Size).FirstOrDefault();
                            if (currentLSStyles != null)
                            {
                                var lsstyle = currentLSStyles.LSStyle;
                                AllocCuttingOutputToLSStyle_Over(cuttingOutput, allocQuantity, lsstyle, userName);
                            }
                            else
                            {
                               
                                if (currentLSStyleForMerge != "")
                                {
                                    AllocCuttingOutputToLSStyle_Over(cuttingOutput, allocQuantity, currentLSStyleForMerge, userName);
                                }
                                else if (currentPriorityLSStyle != "")
                                {
                                    AllocCuttingOutputToLSStyle_Over(cuttingOutput, allocQuantity, currentPriorityLSStyle, userName);
                                }
                                else
                                {
                                    var lastLSStyle = lsStyles.Where(x => x.Size == cuttingOutput.Size).LastOrDefault();
                                    if (lastLSStyle != null)
                                    {
                                        var lsstyle = lastLSStyle.LSStyle;
                                        AllocCuttingOutputToLSStyle_Over(cuttingOutput, allocQuantity, lsstyle, userName);
                                    }
                                }    
                            }
                        }
                    }
                }
                if (allocQuantity == 0)
                {
                    cuttingOutput.IsAllocated = true;
                    cuttingOutput.SetUpdateAudit(userName);
                    context.CuttingOutput.Update(cuttingOutput);
                    context.SaveChanges();
                }
                else
                {
                    cuttingOutput.IsAllocated = true;
                    cuttingOutput.ResidualQuantity = allocQuantity;
                    cuttingOutput.SetUpdateAudit(userName);
                    context.CuttingOutput.Update(cuttingOutput);
                    context.SaveChanges();
                }
                // Create CuttingCard
                CreateCuttingCard(cuttingOutput, userName);
            }
        }
        public void ReturnAllocCuttingOutput(CuttingOutput cuttingOutput,string userName)
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
            //
            var allocTransactions_Lot = context.AllocTransaction.Where(x => x.CuttingOutputID == cuttingOutput.ID
                                                                    && x.IsRetured == false
                                                                    && x.Operation == "CUTTING"
                                                                    //&& x.Size == cuttingOutput.Size
                                                                    && !string.IsNullOrEmpty(x.Lot)
                                                                ).ToList();
            foreach(var allocTransaction in allocTransactions_Lot)
            {
                context.AllocTransaction.Remove(allocTransaction);           
                context.SaveChanges();
            }    
            //
            //reverse Lot
            // insert 09092022
            var removeCuttingLots = context.CuttingLot.Where(x => x.CuttingOutputID == cuttingOutput.ID).ToList();
            foreach (var cuttingLot in removeCuttingLots)
            {
                context.CuttingLot.Remove(cuttingLot);
                context.SaveChanges();
            }
            // cmt 09092022
            
            //remove cutting Card
            var cuttingCards = context.CuttingCard.Where(x => x.CuttingOutputID == cuttingOutput.ID).ToList();
            foreach(var cuttingCard in cuttingCards)
            {
                context.CuttingCard.Remove(cuttingCard);
                context.SaveChanges();
            }    

        }
        public decimal AllocCuttingOutputToLSStyle(CuttingOutput cuttingOutput,decimal quantity,string lsStyle,string userName)
        {
            var curQuantity = quantity;
            var residualQuantity = quantity;
            var Sets = cuttingOutput.Set.Split(";").ToList();
            var fabricContrasts = cuttingOutput.FabricContrast.Split(";").ToList();
            var allocDailyOutputs = new List<AllocDailyOutput>();
            if (cuttingOutput.IsAllSize)
            {
                allocDailyOutputs = context.AllocDailyOutput.Where(x => x.IsFull == false
                                                                && x.IsCanceled == false
                                                                && x.IsDeleted == false
                                                                && x.Operation == "CUTTING"                                                          
                                                                && x.LSStyle == lsStyle  
                                                                && x.FabricContrastName == cuttingOutput.FabricContrast
                                                                ).OrderBy(x => x.Size).ToList();
            }
            else
            {
                allocDailyOutputs = context.AllocDailyOutput.Where(x => x.IsFull == false
                                                                && x.IsCanceled == false
                                                                && x.IsDeleted == false
                                                                && x.Operation == "CUTTING"
                                                                && x.LSStyle == lsStyle                                                                
                                                                && x.Size == cuttingOutput.Size
                                                                && x.FabricContrastName == cuttingOutput.FabricContrast
                                                                ).OrderBy(x => x.Size).ToList();
            }
            if(!cuttingOutput.IsAllSize)
            {
                foreach (var fabricContrast in fabricContrasts)
                {
                    foreach (var set in Sets)
                    {
                        var groupAllocDailyOutputs = allocDailyOutputs.Where(x => x.FabricContrastName == fabricContrast
                                                                            && x.Set == set
                                                                            ).ToList();
                        curQuantity = quantity;
                        foreach (var allocDailyOutput in groupAllocDailyOutputs)
                        {
                            var issueQuantity = (allocDailyOutput.OrderQuantity 
                                + allocDailyOutput.PercentQuantity + allocDailyOutput.Sample) - allocDailyOutput.Quantity;
                            if (issueQuantity <= curQuantity)
                            {
                                allocDailyOutput.Quantity = allocDailyOutput.Quantity + issueQuantity;
                                allocDailyOutput.IsFull = true;
                                residualQuantity = curQuantity - issueQuantity;
                                allocDailyOutput.SetUpdateAudit(userName);
                                context.AllocDailyOutput.Update(allocDailyOutput);


                                var allocTransaction = new AllocTransaction("CUTTING");
                                allocTransaction.CuttingOutputID = cuttingOutput.ID;
                                allocTransaction.AllocDailyOuputID = allocDailyOutput.ID;
                                allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                                allocTransaction.Size = allocDailyOutput.Size;
                                allocTransaction.Set = allocDailyOutput.Set;
                                allocTransaction.FabricContrastName = fabricContrast;
                                allocTransaction.AllocQuantity = issueQuantity;
                                allocTransaction.IsRetured = false;
                                allocTransaction.SetCreateAudit(userName);
                                context.AllocTransaction.Add(allocTransaction);
                                context.SaveChanges();
                                // create Lot

                                if (!string.IsNullOrEmpty(cuttingOutput.Lot))
                                {
                                    var existCuttingLots = context.CuttingLot.Where(x => x.LSStyle == allocDailyOutput.LSStyle
                                                                                    && x.Lot == cuttingOutput.Lot
                                                                                    && x.Size == allocDailyOutput.Size
                                                                                    && x.Set == allocDailyOutput.Set
                                                                                    && x.CuttingOutputID == cuttingOutput.ID
                                                                                    ).ToList();
                                    if (existCuttingLots.Count == 0)
                                    {
                                        var cuttingLot = new CuttingLot();
                                        cuttingLot.Lot = cuttingOutput.Lot;
                                        cuttingLot.Quantity = issueQuantity;
                                        cuttingLot.AllocDailyOutputID = allocDailyOutput.ID;
                                        cuttingLot.LSStyle = allocDailyOutput.LSStyle;
                                        cuttingLot.Size = allocDailyOutput.Size;
                                        cuttingLot.Set = allocDailyOutput.Set;
                                        cuttingLot.CuttingOutputID = cuttingOutput.ID;
                                        cuttingLot.SetCreateAudit(userName);
                                        context.CuttingLot.Add(cuttingLot);

                                        var allocTransaction1 = new AllocTransaction("CUTTING");
                                        allocTransaction1.CuttingOutputID = cuttingOutput.ID;
                                        allocTransaction1.AllocDailyOuputID = allocDailyOutput.ID;
                                        allocTransaction1.LSStyle = allocDailyOutput.LSStyle;
                                        allocTransaction1.Size = allocDailyOutput.Size;
                                        allocTransaction1.Set = allocDailyOutput.Set;
                                        allocTransaction1.FabricContrastName = fabricContrast;
                                        allocTransaction1.Lot = cuttingOutput.Lot;
                                        allocTransaction1.AllocQuantity = issueQuantity;
                                        allocTransaction1.IsRetured = false;
                                        allocTransaction1.SetCreateAudit(userName);
                                        context.AllocTransaction.Add(allocTransaction1);

                                        context.SaveChanges();

                                    }
                                }
                            }
                            else
                            {
                                issueQuantity = curQuantity;
                                curQuantity = 0;
                                residualQuantity = 0;
                                allocDailyOutput.Quantity = allocDailyOutput.Quantity + issueQuantity;
                                allocDailyOutput.SetUpdateAudit(userName);
                                context.AllocDailyOutput.Update(allocDailyOutput);

                                var allocTransaction = new AllocTransaction("CUTTING");
                                allocTransaction.CuttingOutputID = cuttingOutput.ID;
                                allocTransaction.AllocDailyOuputID = allocDailyOutput.ID;
                                allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                                allocTransaction.Size = allocDailyOutput.Size;
                                allocTransaction.Set = allocDailyOutput.Set;
                                allocTransaction.FabricContrastName = fabricContrast;
                                allocTransaction.AllocQuantity = issueQuantity;
                                allocTransaction.IsRetured = false;
                                allocTransaction.SetCreateAudit(userName);
                                context.AllocTransaction.Add(allocTransaction);
                                context.SaveChanges();

                                if (!string.IsNullOrEmpty(cuttingOutput.Lot))
                                {
                                    var existCuttingLots = context.CuttingLot.Where(x => x.LSStyle == allocDailyOutput.LSStyle
                                                                                    && x.Lot == cuttingOutput.Lot
                                                                                    && x.Size == allocDailyOutput.Size
                                                                                    && x.Set == allocDailyOutput.Set
                                                                                    && x.CuttingOutputID == cuttingOutput.ID
                                                                                    ).ToList();
                                    if (existCuttingLots.Count == 0)
                                    {
                                        var cuttingLot = new CuttingLot();
                                        cuttingLot.Lot = cuttingOutput.Lot;
                                        cuttingLot.Quantity = issueQuantity;
                                        cuttingLot.AllocDailyOutputID = allocDailyOutput.ID;
                                        cuttingLot.LSStyle = allocDailyOutput.LSStyle;
                                        cuttingLot.Size = allocDailyOutput.Size;
                                        cuttingLot.Set = allocDailyOutput.Set;
                                        cuttingLot.CuttingOutputID = cuttingOutput.ID;
                                        cuttingLot.SetCreateAudit(userName);
                                        context.CuttingLot.Add(cuttingLot);

                                        var allocTransaction1 = new AllocTransaction("CUTTING");
                                        allocTransaction1.CuttingOutputID = cuttingOutput.ID;
                                        allocTransaction1.AllocDailyOuputID = allocDailyOutput.ID;
                                        allocTransaction1.LSStyle = allocDailyOutput.LSStyle;
                                        allocTransaction1.Size = allocDailyOutput.Size;
                                        allocTransaction1.Set = allocDailyOutput.Set;
                                        allocTransaction1.FabricContrastName = fabricContrast;
                                        allocTransaction1.Lot = cuttingOutput.Lot;
                                        allocTransaction1.AllocQuantity = issueQuantity;
                                        allocTransaction1.IsRetured = false;
                                        allocTransaction1.SetCreateAudit(userName);
                                        context.AllocTransaction.Add(allocTransaction1);

                                        context.SaveChanges();
                                    }
                                }
                                break;
                            }
                        }

                    }

                }
                context.SaveChanges();
            }    
            else
            {

                foreach (var fabricContrast in fabricContrasts)
                {
                    foreach (var set in Sets)
                    {
                        var groupAllocDailyOutputs = allocDailyOutputs.Where(x => x.FabricContrastName == fabricContrast
                                                                            && x.Set == set
                                                                            ).ToList();
                        curQuantity = quantity;
                        residualQuantity = UpdateAllocDailyOutputAllSize(cuttingOutput, groupAllocDailyOutputs, curQuantity, fabricContrast, set, userName);

                    }
                }               
                context.SaveChanges();
            }
            return residualQuantity;
        }
        //for HA
        public decimal AllocCuttingOutputToLSStyle_NotSample(CuttingOutput cuttingOutput, decimal quantity, string lsStyle, string userName)
        {
            var curQuantity = quantity;
            var residualQuantity = quantity;
            var Sets = cuttingOutput.Set.Split(";").ToList();
            var fabricContrasts = cuttingOutput.FabricContrast.Split(";").ToList();
            var allocDailyOutputs = new List<AllocDailyOutput>();
            if (cuttingOutput.IsAllSize)
            {
                allocDailyOutputs = context.AllocDailyOutput.Where(x => x.IsFull == false
                                                                && x.IsCanceled == false
                                                                && x.Operation == "CUTTING"
                                                                && x.LSStyle == lsStyle
                                                                ).OrderBy(x => x.Size).ToList();
            }
            else
            {
                allocDailyOutputs = context.AllocDailyOutput.Where(x => x.IsFull == false
                                                                && x.IsCanceled == false
                                                                && x.Operation == "CUTTING"
                                                                && x.LSStyle == lsStyle
                                                                && x.Size == cuttingOutput.Size
                                                                && x.FabricContrastName == cuttingOutput.FabricContrast
                                                                ).OrderBy(x => x.Size).ToList();
            }
            if (!cuttingOutput.IsAllSize)
            {
                foreach (var fabricContrast in fabricContrasts)
                {
                    foreach (var set in Sets)
                    {
                        var groupAllocDailyOutputs = allocDailyOutputs.Where(x => x.FabricContrastName == fabricContrast
                                                                            && x.Set == set
                                                                            ).ToList();
                        curQuantity = quantity;
                        foreach (var allocDailyOutput in groupAllocDailyOutputs)
                        {
                            var issueQuantity = (allocDailyOutput.OrderQuantity + allocDailyOutput.PercentQuantity) - allocDailyOutput.Quantity;
                            if (issueQuantity <= curQuantity)
                            {
                                allocDailyOutput.Quantity = allocDailyOutput.Quantity + issueQuantity;
                                allocDailyOutput.IsFull = true;
                                residualQuantity = curQuantity - issueQuantity;
                                allocDailyOutput.SetUpdateAudit(userName);
                                context.AllocDailyOutput.Update(allocDailyOutput);


                                var allocTransaction = new AllocTransaction("CUTTING");
                                allocTransaction.CuttingOutputID = cuttingOutput.ID;
                                allocTransaction.AllocDailyOuputID = allocDailyOutput.ID;
                                allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                                allocTransaction.Size = allocDailyOutput.Size;
                                allocTransaction.Set = allocDailyOutput.Set;
                                allocTransaction.FabricContrastName = fabricContrast;
                                allocTransaction.AllocQuantity = issueQuantity;
                                allocTransaction.IsRetured = false;
                                allocTransaction.SetCreateAudit(userName);
                                context.AllocTransaction.Add(allocTransaction);
                                context.SaveChanges();
                                // create Lot

                                if (!string.IsNullOrEmpty(cuttingOutput.Lot))
                                {
                                    var existCuttingLots = context.CuttingLot.Where(x => x.LSStyle == allocDailyOutput.LSStyle
                                                                                    && x.Lot == cuttingOutput.Lot
                                                                                    && x.Size == allocDailyOutput.Size
                                                                                    && x.Set == allocDailyOutput.Set
                                                                                    && x.CuttingOutputID == cuttingOutput.ID
                                                                                    ).ToList();
                                    if (existCuttingLots.Count == 0)
                                    {
                                        var cuttingLot = new CuttingLot();
                                        cuttingLot.Lot = cuttingOutput.Lot;
                                        cuttingLot.Quantity = issueQuantity;
                                        cuttingLot.AllocDailyOutputID = allocDailyOutput.ID;
                                        cuttingLot.LSStyle = allocDailyOutput.LSStyle;
                                        cuttingLot.Size = allocDailyOutput.Size;
                                        cuttingLot.Set = allocDailyOutput.Set;
                                        cuttingLot.CuttingOutputID = cuttingOutput.ID;
                                        cuttingLot.SetCreateAudit(userName);
                                        context.CuttingLot.Add(cuttingLot);

                                        var allocTransaction1 = new AllocTransaction("CUTTING");
                                        allocTransaction1.CuttingOutputID = cuttingOutput.ID;
                                        allocTransaction1.AllocDailyOuputID = allocDailyOutput.ID;
                                        allocTransaction1.LSStyle = allocDailyOutput.LSStyle;
                                        allocTransaction1.Size = allocDailyOutput.Size;
                                        allocTransaction1.Set = allocDailyOutput.Set;
                                        allocTransaction1.FabricContrastName = fabricContrast;
                                        allocTransaction1.Lot = cuttingOutput.Lot;
                                        allocTransaction1.AllocQuantity = issueQuantity;
                                        allocTransaction1.IsRetured = false;
                                        allocTransaction1.SetCreateAudit(userName);
                                        context.AllocTransaction.Add(allocTransaction1);

                                        context.SaveChanges();

                                    }
                                }
                            }
                            else
                            {
                                issueQuantity = curQuantity;
                                curQuantity = 0;
                                residualQuantity = 0;
                                allocDailyOutput.Quantity = allocDailyOutput.Quantity + issueQuantity;
                                allocDailyOutput.SetUpdateAudit(userName);
                                context.AllocDailyOutput.Update(allocDailyOutput);

                                var allocTransaction = new AllocTransaction("CUTTING");
                                allocTransaction.CuttingOutputID = cuttingOutput.ID;
                                allocTransaction.AllocDailyOuputID = allocDailyOutput.ID;
                                allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                                allocTransaction.Size = allocDailyOutput.Size;
                                allocTransaction.Set = allocDailyOutput.Set;
                                allocTransaction.FabricContrastName = fabricContrast;
                                allocTransaction.AllocQuantity = issueQuantity;
                                allocTransaction.IsRetured = false;
                                allocTransaction.SetCreateAudit(userName);
                                context.AllocTransaction.Add(allocTransaction);
                                context.SaveChanges();

                                if (!string.IsNullOrEmpty(cuttingOutput.Lot))
                                {
                                    var existCuttingLots = context.CuttingLot.Where(x => x.LSStyle == allocDailyOutput.LSStyle
                                                                                    && x.Lot == cuttingOutput.Lot
                                                                                    && x.Size == allocDailyOutput.Size
                                                                                    && x.Set == allocDailyOutput.Set
                                                                                    && x.CuttingOutputID == cuttingOutput.ID
                                                                                    ).ToList();
                                    if (existCuttingLots.Count == 0)
                                    {
                                        var cuttingLot = new CuttingLot();
                                        cuttingLot.Lot = cuttingOutput.Lot;
                                        cuttingLot.Quantity = issueQuantity;
                                        cuttingLot.AllocDailyOutputID = allocDailyOutput.ID;
                                        cuttingLot.LSStyle = allocDailyOutput.LSStyle;
                                        cuttingLot.Size = allocDailyOutput.Size;
                                        cuttingLot.Set = allocDailyOutput.Set;
                                        cuttingLot.CuttingOutputID = cuttingOutput.ID;
                                        cuttingLot.SetCreateAudit(userName);
                                        context.CuttingLot.Add(cuttingLot);

                                        var allocTransaction1 = new AllocTransaction("CUTTING");
                                        allocTransaction1.CuttingOutputID = cuttingOutput.ID;
                                        allocTransaction1.AllocDailyOuputID = allocDailyOutput.ID;
                                        allocTransaction1.LSStyle = allocDailyOutput.LSStyle;
                                        allocTransaction1.Size = allocDailyOutput.Size;
                                        allocTransaction1.Set = allocDailyOutput.Set;
                                        allocTransaction1.FabricContrastName = fabricContrast;
                                        allocTransaction1.Lot = cuttingOutput.Lot;
                                        allocTransaction1.AllocQuantity = issueQuantity;
                                        allocTransaction1.IsRetured = false;
                                        allocTransaction1.SetCreateAudit(userName);
                                        context.AllocTransaction.Add(allocTransaction1);

                                        context.SaveChanges();
                                    }
                                }
                                break;
                            }
                        }

                    }

                }
                context.SaveChanges();
            }
            else
            {

                foreach (var fabricContrast in fabricContrasts)
                {
                    foreach (var set in Sets)
                    {
                        var groupAllocDailyOutputs = allocDailyOutputs.Where(x => x.FabricContrastName == fabricContrast
                                                                            && x.Set == set
                                                                            ).ToList();
                        curQuantity = quantity;
                        residualQuantity = UpdateAllocDailyOutputAllSize(cuttingOutput, groupAllocDailyOutputs, curQuantity, fabricContrast, set, userName);

                    }

                }

                context.SaveChanges();
            }

            return residualQuantity;
        }
        // For HA
        public decimal AllocCuttingOutputToLSStyle_Sample(CuttingOutput cuttingOutput, decimal quantity, string lsStyle, string userName)
        {
            var curQuantity = quantity;
            var residualQuantity = quantity;
            var Sets = cuttingOutput.Set.Split(";").ToList();
            var fabricContrasts = cuttingOutput.FabricContrast.Split(";").ToList();
            var allocDailyOutputs = new List<AllocDailyOutput>();
            if (cuttingOutput.IsAllSize)
            {
                allocDailyOutputs = context.AllocDailyOutput.Where(x => x.IsFull == true
                                                                && x.IsCanceled == false
                                                                && x.Operation == "CUTTING"
                                                                && x.LSStyle == lsStyle
                                                                ).OrderBy(x => x.Size).ToList();
            }
            else
            {
                allocDailyOutputs = context.AllocDailyOutput.Where(x => x.IsFull == true
                                                                && x.IsCanceled == false
                                                                && x.Operation == "CUTTING"
                                                                && x.LSStyle == lsStyle
                                                                && x.Size == cuttingOutput.Size
                                                                && x.FabricContrastName == cuttingOutput.FabricContrast
                                                                ).OrderBy(x => x.Size).ToList();
            }
            if (!cuttingOutput.IsAllSize)
            {
                foreach (var fabricContrast in fabricContrasts)
                {
                    foreach (var set in Sets)
                    {
                        var groupAllocDailyOutputs = allocDailyOutputs.Where(x => x.FabricContrastName == fabricContrast
                                                                            && x.Set == set
                                                                            ).ToList();
                        curQuantity = quantity;
                        foreach (var allocDailyOutput in groupAllocDailyOutputs)
                        {
                            if(allocDailyOutput.Quantity< allocDailyOutput.OrderQuantity + allocDailyOutput.Sample)
                            {
                                var issueQuantity = allocDailyOutput.Sample;
                                if (issueQuantity <= curQuantity)
                                {
                                    allocDailyOutput.Quantity = allocDailyOutput.Quantity + issueQuantity;
                                    allocDailyOutput.IsFull = true;
                                    residualQuantity = curQuantity - issueQuantity;
                                    allocDailyOutput.SetUpdateAudit(userName);
                                    context.AllocDailyOutput.Update(allocDailyOutput);


                                    var allocTransaction = new AllocTransaction("CUTTING");
                                    allocTransaction.CuttingOutputID = cuttingOutput.ID;
                                    allocTransaction.AllocDailyOuputID = allocDailyOutput.ID;
                                    allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                                    allocTransaction.Size = allocDailyOutput.Size;
                                    allocTransaction.Set = allocDailyOutput.Set;
                                    allocTransaction.FabricContrastName = fabricContrast;
                                    allocTransaction.AllocQuantity = issueQuantity;
                                    allocTransaction.IsRetured = false;
                                    allocTransaction.SetCreateAudit(userName);
                                    context.AllocTransaction.Add(allocTransaction);
                                    context.SaveChanges();
                                    // create Lot

                                    if (!string.IsNullOrEmpty(cuttingOutput.Lot))
                                    {
                                        var existCuttingLots = context.CuttingLot.Where(x => x.LSStyle == allocDailyOutput.LSStyle
                                                                                        && x.Lot == cuttingOutput.Lot
                                                                                        && x.Size == allocDailyOutput.Size
                                                                                        && x.Set == allocDailyOutput.Set
                                                                                        && x.CuttingOutputID == cuttingOutput.ID
                                                                                        ).ToList();
                                        if (existCuttingLots.Count == 0)
                                        {
                                            var cuttingLot = new CuttingLot();
                                            cuttingLot.Lot = cuttingOutput.Lot;
                                            cuttingLot.Quantity = issueQuantity;
                                            cuttingLot.AllocDailyOutputID = allocDailyOutput.ID;
                                            cuttingLot.LSStyle = allocDailyOutput.LSStyle;
                                            cuttingLot.Size = allocDailyOutput.Size;
                                            cuttingLot.Set = allocDailyOutput.Set;
                                            cuttingLot.CuttingOutputID = cuttingOutput.ID;
                                            cuttingLot.SetCreateAudit(userName);
                                            context.CuttingLot.Add(cuttingLot);

                                            var allocTransaction1 = new AllocTransaction("CUTTING");
                                            allocTransaction1.CuttingOutputID = cuttingOutput.ID;
                                            allocTransaction1.AllocDailyOuputID = allocDailyOutput.ID;
                                            allocTransaction1.LSStyle = allocDailyOutput.LSStyle;
                                            allocTransaction1.Size = allocDailyOutput.Size;
                                            allocTransaction1.Set = allocDailyOutput.Set;
                                            allocTransaction1.FabricContrastName = fabricContrast;
                                            allocTransaction1.Lot = cuttingOutput.Lot;
                                            allocTransaction1.AllocQuantity = issueQuantity;
                                            allocTransaction1.IsRetured = false;
                                            allocTransaction1.SetCreateAudit(userName);
                                            context.AllocTransaction.Add(allocTransaction1);

                                            context.SaveChanges();

                                        }
                                    }
                                }
                                else
                                {
                                    issueQuantity = curQuantity;
                                    curQuantity = 0;
                                    residualQuantity = 0;
                                    allocDailyOutput.Quantity = allocDailyOutput.Quantity + issueQuantity;
                                    allocDailyOutput.SetUpdateAudit(userName);
                                    context.AllocDailyOutput.Update(allocDailyOutput);

                                    var allocTransaction = new AllocTransaction("CUTTING");
                                    allocTransaction.CuttingOutputID = cuttingOutput.ID;
                                    allocTransaction.AllocDailyOuputID = allocDailyOutput.ID;
                                    allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                                    allocTransaction.Size = allocDailyOutput.Size;
                                    allocTransaction.Set = allocDailyOutput.Set;
                                    allocTransaction.FabricContrastName = fabricContrast;
                                    allocTransaction.AllocQuantity = issueQuantity;
                                    allocTransaction.IsRetured = false;
                                    allocTransaction.SetCreateAudit(userName);
                                    context.AllocTransaction.Add(allocTransaction);
                                    context.SaveChanges();

                                    if (!string.IsNullOrEmpty(cuttingOutput.Lot))
                                    {
                                        var existCuttingLots = context.CuttingLot.Where(x => x.LSStyle == allocDailyOutput.LSStyle
                                                                                        && x.Lot == cuttingOutput.Lot
                                                                                        && x.Size == allocDailyOutput.Size
                                                                                        && x.Set == allocDailyOutput.Set
                                                                                        && x.CuttingOutputID == cuttingOutput.ID
                                                                                        ).ToList();
                                        if (existCuttingLots.Count == 0)
                                        {
                                            var cuttingLot = new CuttingLot();
                                            cuttingLot.Lot = cuttingOutput.Lot;
                                            cuttingLot.Quantity = issueQuantity;
                                            cuttingLot.AllocDailyOutputID = allocDailyOutput.ID;
                                            cuttingLot.LSStyle = allocDailyOutput.LSStyle;
                                            cuttingLot.Size = allocDailyOutput.Size;
                                            cuttingLot.Set = allocDailyOutput.Set;
                                            cuttingLot.CuttingOutputID = cuttingOutput.ID;
                                            cuttingLot.SetCreateAudit(userName);
                                            context.CuttingLot.Add(cuttingLot);

                                            var allocTransaction1 = new AllocTransaction("CUTTING");
                                            allocTransaction1.CuttingOutputID = cuttingOutput.ID;
                                            allocTransaction1.AllocDailyOuputID = allocDailyOutput.ID;
                                            allocTransaction1.LSStyle = allocDailyOutput.LSStyle;
                                            allocTransaction1.Size = allocDailyOutput.Size;
                                            allocTransaction1.Set = allocDailyOutput.Set;
                                            allocTransaction1.FabricContrastName = fabricContrast;
                                            allocTransaction1.Lot = cuttingOutput.Lot;
                                            allocTransaction1.AllocQuantity = issueQuantity;
                                            allocTransaction1.IsRetured = false;
                                            allocTransaction1.SetCreateAudit(userName);
                                            context.AllocTransaction.Add(allocTransaction1);

                                            context.SaveChanges();
                                        }
                                    }
                                    break;
                                }
                            }    
                            
                        }

                    }

                }
                context.SaveChanges();
            }
            else
            {

                foreach (var fabricContrast in fabricContrasts)
                {
                    foreach (var set in Sets)
                    {
                        var groupAllocDailyOutputs = allocDailyOutputs.Where(x => x.FabricContrastName == fabricContrast
                                                                            && x.Set == set
                                                                            ).ToList();
                        curQuantity = quantity;
                        residualQuantity = UpdateAllocDailyOutputAllSize(cuttingOutput, groupAllocDailyOutputs, curQuantity, fabricContrast, set, userName);

                    }

                }

                context.SaveChanges();
            }

            return residualQuantity;
        }
        public decimal AllocCuttingOutputToLSStyle_Over(CuttingOutput cuttingOutput, decimal quantity, string lsStyle, string userName)
        {
            var curQuantity = quantity;
            var residualQuantity = quantity;
            var Sets = cuttingOutput.Set.Split(";").ToList();
            var fabricContrasts = cuttingOutput.FabricContrast.Split(";").ToList();
            var allocDailyOutputs = new List<AllocDailyOutput>();
            if (cuttingOutput.IsAllSize)
            {
                allocDailyOutputs = context.AllocDailyOutput.Where(x => x.IsFull == true
                                                                && x.IsCanceled == false
                                                                && x.Operation == "CUTTING"
                                                                && x.LSStyle == lsStyle
                                                                ).OrderBy(x => x.Size).ToList();
            }
            else
            {
                allocDailyOutputs = context.AllocDailyOutput.Where(x => x.IsFull == true
                                                                && x.IsCanceled == false
                                                                && x.Operation == "CUTTING"
                                                                && x.LSStyle == lsStyle
                                                                && x.Size == cuttingOutput.Size
                                                                && x.FabricContrastName == cuttingOutput.FabricContrast
                                                                ).OrderBy(x => x.Size).ToList();
            }
            if (!cuttingOutput.IsAllSize)
            {
                foreach (var fabricContrast in fabricContrasts)
                {
                    foreach (var set in Sets)
                    {
                        var groupAllocDailyOutputs = allocDailyOutputs.Where(x => x.FabricContrastName == fabricContrast
                                                                            && x.Set == set
                                                                            ).ToList();
                        curQuantity = quantity;
                        foreach (var allocDailyOutput in groupAllocDailyOutputs)
                        {
                            var issueQuantity = quantity;
                            if (issueQuantity <= curQuantity)
                            {
                                allocDailyOutput.Quantity = allocDailyOutput.Quantity + issueQuantity;
                                allocDailyOutput.IsFull = true;
                                residualQuantity = curQuantity - issueQuantity;
                                allocDailyOutput.SetUpdateAudit(userName);
                                context.AllocDailyOutput.Update(allocDailyOutput);


                                var allocTransaction = new AllocTransaction("CUTTING");
                                allocTransaction.CuttingOutputID = cuttingOutput.ID;
                                allocTransaction.AllocDailyOuputID = allocDailyOutput.ID;
                                allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                                allocTransaction.Size = allocDailyOutput.Size;
                                allocTransaction.Set = allocDailyOutput.Set;
                                allocTransaction.FabricContrastName = fabricContrast;
                                allocTransaction.AllocQuantity = issueQuantity;
                                allocTransaction.IsRetured = false;                                
                                allocTransaction.SetCreateAudit(userName);
                                context.AllocTransaction.Add(allocTransaction);
                                // create Lot

                                if (!string.IsNullOrEmpty(cuttingOutput.Lot))
                                {
                                    // Cmt: 09092022 - No check exist when alloc over
                                    //var existCuttingLots = context.CuttingLot.Where(x => x.LSStyle == allocDailyOutput.LSStyle
                                    //                                                && x.Lot == cuttingOutput.Lot
                                    //                                                && x.Size == allocDailyOutput.Size
                                    //                                                && x.Set == allocDailyOutput.Set
                                    //                                                && x.CuttingOutputID == cuttingOutput.ID
                                    //                                                ).ToList();
                                    //if (existCuttingLots.Count == 0)
                                    //{
                                        var cuttingLot = new CuttingLot();
                                        cuttingLot.Lot = cuttingOutput.Lot;
                                        cuttingLot.Quantity = issueQuantity;
                                        cuttingLot.AllocDailyOutputID = allocDailyOutput.ID;
                                        cuttingLot.LSStyle = allocDailyOutput.LSStyle;
                                        cuttingLot.Size = allocDailyOutput.Size;
                                        cuttingLot.Set = allocDailyOutput.Set;
                                        cuttingLot.CuttingOutputID = cuttingOutput.ID;
                                        cuttingLot.SetCreateAudit(userName);
                                        context.CuttingLot.Add(cuttingLot);

                                        var allocTransaction1 = new AllocTransaction("CUTTING");
                                        allocTransaction1.CuttingOutputID = cuttingOutput.ID;
                                        allocTransaction1.AllocDailyOuputID = allocDailyOutput.ID;
                                        allocTransaction1.LSStyle = allocDailyOutput.LSStyle;
                                        allocTransaction1.Size = allocDailyOutput.Size;
                                        allocTransaction1.Set = allocDailyOutput.Set;
                                        allocTransaction1.FabricContrastName = fabricContrast;
                                        allocTransaction1.Lot = cuttingOutput.Lot;
                                        allocTransaction1.AllocQuantity = issueQuantity;
                                        allocTransaction1.IsRetured = false;
                                        allocTransaction1.SetCreateAudit(userName);
                                        context.AllocTransaction.Add(allocTransaction1);

                                        context.SaveChanges();

                                    //}
                                }
                            }
                            else
                            {
                                issueQuantity = curQuantity;
                                curQuantity = 0;
                                residualQuantity = 0;
                                allocDailyOutput.Quantity = allocDailyOutput.Quantity + issueQuantity;
                                allocDailyOutput.SetUpdateAudit(userName);
                                context.AllocDailyOutput.Update(allocDailyOutput);

                                var allocTransaction = new AllocTransaction("CUTTING");
                                allocTransaction.CuttingOutputID = cuttingOutput.ID;
                                allocTransaction.AllocDailyOuputID = allocDailyOutput.ID;
                                allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                                allocTransaction.Size = allocDailyOutput.Size;
                                allocTransaction.Set = allocDailyOutput.Set;
                                allocTransaction.FabricContrastName = fabricContrast;
                                allocTransaction.AllocQuantity = issueQuantity;
                                allocTransaction.IsRetured = false;
                                allocTransaction.SetCreateAudit(userName);
                                context.AllocTransaction.Add(allocTransaction);

                                if (!string.IsNullOrEmpty(cuttingOutput.Lot))
                                {
                                    var existCuttingLots = context.CuttingLot.Where(x => x.LSStyle == allocDailyOutput.LSStyle
                                                                                    && x.Lot == cuttingOutput.Lot
                                                                                    && x.Size == allocDailyOutput.Size
                                                                                    && x.Set == allocDailyOutput.Set
                                                                                    && x.CuttingOutputID == cuttingOutput.ID
                                                                                    ).ToList();
                                    if (existCuttingLots.Count == 0)
                                    {
                                        var cuttingLot = new CuttingLot();
                                        cuttingLot.Lot = cuttingOutput.Lot;
                                        cuttingLot.Quantity = issueQuantity;
                                        cuttingLot.AllocDailyOutputID = allocDailyOutput.ID;
                                        cuttingLot.LSStyle = allocDailyOutput.LSStyle;
                                        cuttingLot.Size = allocDailyOutput.Size;
                                        cuttingLot.Set = allocDailyOutput.Set;
                                        cuttingLot.CuttingOutputID = cuttingOutput.ID;
                                        cuttingLot.SetCreateAudit(userName);
                                        context.CuttingLot.Add(cuttingLot);

                                        var allocTransaction1 = new AllocTransaction("CUTTING");
                                        allocTransaction1.CuttingOutputID = cuttingOutput.ID;
                                        allocTransaction1.AllocDailyOuputID = allocDailyOutput.ID;
                                        allocTransaction1.LSStyle = allocDailyOutput.LSStyle;
                                        allocTransaction1.Size = allocDailyOutput.Size;
                                        allocTransaction1.Set = allocDailyOutput.Set;
                                        allocTransaction1.FabricContrastName = fabricContrast;
                                        allocTransaction1.Lot = cuttingOutput.Lot;
                                        allocTransaction1.AllocQuantity = issueQuantity;
                                        allocTransaction1.IsRetured = false;
                                        allocTransaction1.SetCreateAudit(userName);
                                        context.AllocTransaction.Add(allocTransaction1);

                                        context.SaveChanges();
                                    }
                                }
                                break;
                            }
                        }

                    }

                }
                context.SaveChanges();
            }
            else
            {

                foreach (var fabricContrast in fabricContrasts)
                {
                    foreach (var set in Sets)
                    {
                        var lastAllocDailyOutput = allocDailyOutputs.Where(x => x.FabricContrastName == fabricContrast
                                                                            && x.Set == set
                                                                            ).ToList().Last();
                        var groupAllocDailyOutputs = new List<AllocDailyOutput>();
                        groupAllocDailyOutputs.Add(lastAllocDailyOutput);
                        curQuantity = quantity;
                        residualQuantity = UpdateAllocDailyOutputAllSize_Over(cuttingOutput, groupAllocDailyOutputs, curQuantity, fabricContrast, set, userName);

                    }

                }

                context.SaveChanges();
            }

            return residualQuantity;
        }
        public decimal UpdateAllocDailyOutputAllSize(CuttingOutput cuttingOutput,List<AllocDailyOutput> groupAllocDailyOutputs,
            decimal quantity,string fabricContrast, string Set,string userName)
        {
            var curQuantity = quantity;
            foreach (var allocDailyOutput in groupAllocDailyOutputs)
            {
                var issueQuantity = (allocDailyOutput.OrderQuantity + allocDailyOutput.PercentQuantity) - allocDailyOutput.Quantity;
                if (issueQuantity <= curQuantity)
                {
                    allocDailyOutput.Quantity = allocDailyOutput.Quantity + issueQuantity;
                    allocDailyOutput.IsFull = true;
                    curQuantity = curQuantity - issueQuantity;
                    allocDailyOutput.SetUpdateAudit(userName);
                    context.AllocDailyOutput.Update(allocDailyOutput);


                    var allocTransaction = new AllocTransaction("CUTTING");
                    allocTransaction.CuttingOutputID = cuttingOutput.ID;
                    allocTransaction.AllocDailyOuputID = allocDailyOutput.ID;
                    allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                    allocTransaction.Size = allocDailyOutput.Size;
                    allocTransaction.Set = allocDailyOutput.Set;
                    allocTransaction.FabricContrastName = fabricContrast;
                    allocTransaction.AllocQuantity = issueQuantity;
                    allocTransaction.IsRetured = false;
                    allocTransaction.SetCreateAudit(userName);
                    context.AllocTransaction.Add(allocTransaction);

                    context.SaveChanges();
                    // create Lot

                    if (!string.IsNullOrEmpty(cuttingOutput.Lot))
                    {
                        var existCuttingLots = context.CuttingLot.Where(x => x.LSStyle == allocDailyOutput.LSStyle
                                                                        && x.Lot == cuttingOutput.Lot
                                                                        && x.Size == allocDailyOutput.Size
                                                                        && x.Set == allocDailyOutput.Set
                                                                        && x.CuttingOutputID == cuttingOutput.ID
                                                                        ).ToList();
                        if (existCuttingLots.Count == 0)
                        {
                            var cuttingLot = new CuttingLot();
                            cuttingLot.Lot = cuttingOutput.Lot;
                            cuttingLot.Quantity = issueQuantity;
                            cuttingLot.AllocDailyOutputID = allocDailyOutput.ID;
                            cuttingLot.LSStyle = allocDailyOutput.LSStyle;
                            cuttingLot.Size = allocDailyOutput.Size;
                            cuttingLot.Set = allocDailyOutput.Set;
                            cuttingLot.CuttingOutputID = cuttingOutput.ID;
                            cuttingLot.SetCreateAudit(userName);
                            context.CuttingLot.Add(cuttingLot);

                            var allocTransaction1 = new AllocTransaction("CUTTING");
                            allocTransaction1.CuttingOutputID = cuttingOutput.ID;
                            allocTransaction1.AllocDailyOuputID = allocDailyOutput.ID;
                            allocTransaction1.LSStyle = allocDailyOutput.LSStyle;
                            allocTransaction1.Size = allocDailyOutput.Size;
                            allocTransaction1.Set = allocDailyOutput.Set;
                            allocTransaction1.FabricContrastName = fabricContrast;
                            allocTransaction1.Lot = cuttingOutput.Lot;
                            allocTransaction1.AllocQuantity = issueQuantity;
                            allocTransaction1.IsRetured = false;
                            allocTransaction1.SetCreateAudit(userName);
                            context.AllocTransaction.Add(allocTransaction1);

                            context.SaveChanges();

                        }
                    }
                }
                else
                {
                    issueQuantity = curQuantity;
                    curQuantity = 0;
                 
                    allocDailyOutput.Quantity = allocDailyOutput.Quantity + issueQuantity;
                    allocDailyOutput.SetUpdateAudit(userName);
                    context.AllocDailyOutput.Update(allocDailyOutput);

                    var allocTransaction = new AllocTransaction("CUTTING");
                    allocTransaction.CuttingOutputID = cuttingOutput.ID;
                    allocTransaction.AllocDailyOuputID = allocDailyOutput.ID;
                    allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                    allocTransaction.Size = allocDailyOutput.Size;
                    allocTransaction.Set = allocDailyOutput.Set;
                    allocTransaction.FabricContrastName = fabricContrast;
                    allocTransaction.AllocQuantity = issueQuantity;
                    allocTransaction.IsRetured = false;
                    allocTransaction.SetCreateAudit(userName);
                    context.AllocTransaction.Add(allocTransaction);

                    context.SaveChanges();

                    if (!string.IsNullOrEmpty(cuttingOutput.Lot))
                    {
                        var existCuttingLots = context.CuttingLot.Where(x => x.LSStyle == allocDailyOutput.LSStyle
                                                                        && x.Lot == cuttingOutput.Lot
                                                                        && x.Size == allocDailyOutput.Size
                                                                        && x.Set == allocDailyOutput.Set
                                                                        && x.CuttingOutputID == cuttingOutput.ID
                                                                        ).ToList();
                        if (existCuttingLots.Count == 0)
                        {
                            var cuttingLot = new CuttingLot();
                            cuttingLot.Lot = cuttingOutput.Lot;
                            cuttingLot.Quantity = issueQuantity;
                            cuttingLot.AllocDailyOutputID = allocDailyOutput.ID;
                            cuttingLot.LSStyle = allocDailyOutput.LSStyle;
                            cuttingLot.Size = allocDailyOutput.Size;
                            cuttingLot.Set = allocDailyOutput.Set;
                            cuttingLot.CuttingOutputID = cuttingOutput.ID;
                            cuttingLot.SetCreateAudit(userName);
                            context.CuttingLot.Add(cuttingLot);

                            var allocTransaction1 = new AllocTransaction("CUTTING");
                            allocTransaction1.CuttingOutputID = cuttingOutput.ID;
                            allocTransaction1.AllocDailyOuputID = allocDailyOutput.ID;
                            allocTransaction1.LSStyle = allocDailyOutput.LSStyle;
                            allocTransaction1.Size = allocDailyOutput.Size;
                            allocTransaction1.Set = allocDailyOutput.Set;
                            allocTransaction1.FabricContrastName = fabricContrast;
                            allocTransaction1.Lot = cuttingOutput.Lot;
                            allocTransaction1.AllocQuantity = issueQuantity;
                            allocTransaction1.IsRetured = false;
                            allocTransaction1.SetCreateAudit(userName);
                            context.AllocTransaction.Add(allocTransaction1);

                            context.SaveChanges();
                        }
                    }
                    break;
                }
            }
            return curQuantity;
        }

        public decimal UpdateAllocDailyOutputAllSize_Over(CuttingOutput cuttingOutput,
            List<AllocDailyOutput> groupAllocDailyOutputs, decimal quantity, string fabricContrast, string Set, string userName)
        {
            var curQuantity = quantity;
            foreach (var allocDailyOutput in groupAllocDailyOutputs)
            {
                var issueQuantity = quantity;
                
                allocDailyOutput.Quantity = allocDailyOutput.Quantity + issueQuantity;
                allocDailyOutput.IsFull = true;
                curQuantity = curQuantity - issueQuantity;
                allocDailyOutput.SetUpdateAudit(userName);
                context.AllocDailyOutput.Update(allocDailyOutput);


                var allocTransaction = new AllocTransaction("CUTTING");
                allocTransaction.CuttingOutputID = cuttingOutput.ID;
                allocTransaction.AllocDailyOuputID = allocDailyOutput.ID;
                allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                allocTransaction.Size = allocDailyOutput.Size;
                allocTransaction.Set = allocDailyOutput.Set;
                allocTransaction.FabricContrastName = fabricContrast;
                allocTransaction.AllocQuantity = issueQuantity;
                allocTransaction.IsRetured = false;
                allocTransaction.SetCreateAudit(userName);
                context.AllocTransaction.Add(allocTransaction);

                context.SaveChanges();
                // create Lot

                if (!string.IsNullOrEmpty(cuttingOutput.Lot))
                {
                    var existCuttingLots = context.CuttingLot.Where(x => x.LSStyle == allocDailyOutput.LSStyle
                                                                    && x.Lot == cuttingOutput.Lot
                                                                    && x.Size == allocDailyOutput.Size
                                                                    && x.Set == allocDailyOutput.Set
                                                                    && x.CuttingOutputID == cuttingOutput.ID
                                                                    ).ToList();
                    if (existCuttingLots.Count == 0)
                    {
                        var cuttingLot = new CuttingLot();
                        cuttingLot.Lot = cuttingOutput.Lot;
                        cuttingLot.Quantity = issueQuantity;
                        cuttingLot.AllocDailyOutputID = allocDailyOutput.ID;
                        cuttingLot.LSStyle = allocDailyOutput.LSStyle;
                        cuttingLot.Size = allocDailyOutput.Size;
                        cuttingLot.Set = allocDailyOutput.Set;
                        cuttingLot.CuttingOutputID = cuttingOutput.ID;
                        cuttingLot.SetCreateAudit(userName);
                        context.CuttingLot.Add(cuttingLot);

                        var allocTransaction1 = new AllocTransaction("CUTTING");
                        allocTransaction1.CuttingOutputID = cuttingOutput.ID;
                        allocTransaction1.AllocDailyOuputID = allocDailyOutput.ID;
                        allocTransaction1.LSStyle = allocDailyOutput.LSStyle;
                        allocTransaction1.Size = allocDailyOutput.Size;
                        allocTransaction1.Set = allocDailyOutput.Set;
                        allocTransaction1.FabricContrastName = fabricContrast;
                        allocTransaction1.Lot = cuttingOutput.Lot;
                        allocTransaction1.AllocQuantity = issueQuantity;
                        allocTransaction1.IsRetured = false;
                        allocTransaction1.SetCreateAudit(userName);
                        context.AllocTransaction.Add(allocTransaction1);

                        context.SaveChanges();

                    }
                }  
            }
            return curQuantity;
        }
        public void CreateCuttingCard(CuttingOutput cuttingOutput,string userName)
        {
            var cuttingCards = (from atr in context.AllocTransaction.Where(atr => atr.CuttingOutputID == cuttingOutput.ID
                                                                && atr.Lot == null
                                                                && atr.Operation == "CUTTING")
                          from a in context.AllocDailyOutput.Where(a => a.LSStyle == atr.LSStyle
                                                                  && a.Operation == "CUTTING"
                                                                  && a.Size == atr.Size
                                                                  && a.Set == atr.Set
                                                                  && a.FabricContrastName == atr.FabricContrastName)
                          from d in context.DailyTarget.Where(d => d.LSStyle == a.LSStyle && d.Operation == "CUTTING")
                          from c in context.CuttingOutput.Where(c => c.ID == cuttingOutput.ID)
                          from f in context.FabricContrast.Where(f => f.ID == a.FabricContrastID)
                          select new 
                          {
                              d.MergeBlockLSStyle,
                              d.MergeLSStyle,                             
                              d.LSStyle,
                              d.Season,
                              d.CustomerID,
                              atr.Size,
                              a.Set,
                              a.FabricContrastName,
                              f.ContrastColor,
                              f.Description,
                              atr.AllocQuantity,
                              c.Quantity,
                              c.ProduceDate,
                              c.WorkCenterName,
                              c.TableNO,
                              c.FabricContrastID,
                              c.ID,
                              c.Lot,
                              c.TotalPackage
                              
                          }).AsEnumerable()
                       .GroupBy(g => new
                       {
                           
                           g.MergeBlockLSStyle,
                           g.MergeLSStyle, 
                           g.Season,
                           g.CustomerID,
                           g.Size,
                           g.Set,
                           g.FabricContrastName,
                           g.ContrastColor,
                           g.Description,                           
                           g.Quantity,
                           g.ProduceDate,
                           g.WorkCenterName,
                           g.TableNO,
                           g.FabricContrastID,
                           g.ID,
                           g.Lot,
                           g.TotalPackage
                       })
                       .Select(s => new CuttingCard
                       {
                           MergeBlockLSStyle = s.Key.MergeBlockLSStyle,
                           MergeLSStyle = s.Key.MergeLSStyle,                          
                           Size = s.Key.Size,
                           Season = s.Key.Season,
                           CustomerID = s.Key.CustomerID,
                           Set = s.Key.Set,
                           FabricContrastName = s.Key.FabricContrastName,
                           FabricContrastColor = s.Key.ContrastColor,
                           FabricContrastDescription = s.Key.Description,   
                           Quantity = s.Key.Quantity,
                           ProduceDate = s.Key.ProduceDate,
                           WorkCenterName = s.Key.WorkCenterName,
                           TableNO = s.Key.TableNO,
                           FabricContrastID = s.Key.FabricContrastID,
                           CuttingOutputID = s.Key.ID,
                           Lot = s.Key.Lot,
                           TotalPackage = (int)s.Key.TotalPackage,
                           AllocQuantity = s.Sum(t => t.AllocQuantity)
                       }
                       ).ToList();
            foreach(var cuttingCard in cuttingCards)
            {
                var fabricContrast = context.FabricContrast.Where(f => f.ID == cuttingCard.FabricContrastID).FirstOrDefault();
                cuttingCard.Operation = "CUTTING";
                cuttingCard.CardType = "MASTER";
                if(cuttingCard.Set == "ÁO")
                {
                    cuttingCard.FabricContrastDescription = string.IsNullOrEmpty(fabricContrast.DescriptionForShirt) ? cuttingCard.FabricContrastDescription : fabricContrast.DescriptionForShirt;
                }   
                else if(cuttingCard.Set == "QUẦN")
                {
                    cuttingCard.FabricContrastDescription = string.IsNullOrEmpty(fabricContrast.DescriptionForPant) ? cuttingCard.FabricContrastDescription : fabricContrast.DescriptionForPant;
                }    
                cuttingCard.OnHandQuantity = cuttingCard.AllocQuantity;
                cuttingCard.SetCreateAudit(userName);
                context.CuttingCard.Add(cuttingCard);
                context.SaveChanges();
            }
            
        }
        public void CreateCuttingCardPerCentPrint(CuttingOutput cuttingOutput, string userName)
        {
            var cuttingCard = new CuttingCard();
            mapper.Map(cuttingOutput,cuttingCard);
            cuttingCard.CuttingOutputID = cuttingOutput.ID;
            cuttingCard.AllocQuantity = cuttingCard.OnHandQuantity = cuttingCard.Quantity;
            cuttingCard.Operation = "CUTTING";
            cuttingCard.CardType = "MASTER";
            cuttingCard.FabricContrastName = cuttingOutput.FabricContrast;
            cuttingCard.FabricContrastColor = context.FabricContrast.Where(x => x.ID == cuttingOutput.FabricContrastID).Select(x => x.ContrastColor).FirstOrDefault();
            cuttingCard.MergeLSStyle = cuttingOutput.Remark;
            cuttingCard.SetCreateAudit(userName);
            context.CuttingCard.Add(cuttingCard);
            context.SaveChanges();
        }
        public void AllocCuttingOutputPercentPrint(CuttingOutput cuttingOutput, string userName)
        {
            if(cuttingOutput.IsPrint)
            {
                //return if exist
                ReturnAllocCuttingOutputPercentPrint(cuttingOutput, userName);

                //var strLSStyle = !string.IsNullOrEmpty(cuttingOutput.MergeLSStyle) ? cuttingOutput.MergeLSStyle : cuttingOutput.Remark;
                var lsStyle = cuttingOutput.PriorityLSStyle;
                if(!string.IsNullOrWhiteSpace(lsStyle))
                {
                    //var listLSStyle = getLSStyle(cuttingOutput.MergeBlockLSStyle, strLSStyle);
                    //foreach(var lsStyle in listLSStyle)
                    //{
                    if(cuttingOutput.IsAllSize)
                    {
                        var allocDailyOutputs = context.AllocDailyOutput.Where(x=>x.LSStyle==lsStyle && x.FabricContrastName == cuttingOutput.FabricContrast).ToList();
                        foreach(var allocDailyOutput in allocDailyOutputs)
                        {
                            allocDailyOutput.PercentPrint += cuttingOutput.Quantity;
                            allocDailyOutput.SetUpdateAudit(userName);
                            context.AllocDailyOutput.Update(allocDailyOutput);
                               
                            //create AllocTransaction
                            var allocTransaction = new AllocTransaction("CUTTING");
                            allocTransaction.CuttingOutputID = cuttingOutput.ID;
                            allocTransaction.AllocDailyOuputID = allocDailyOutput.ID;
                            allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                            allocTransaction.Size = allocDailyOutput.Size;
                            allocTransaction.Set = allocDailyOutput.Set;
                            allocTransaction.FabricContrastName = allocDailyOutput.FabricContrastName;
                            allocTransaction.AllocQuantity = cuttingOutput.Quantity;
                            allocTransaction.IsRetured = false;
                            allocTransaction.SetCreateAudit(userName);
                            context.AllocTransaction.Add(allocTransaction);
                            context.SaveChanges();
                        }    
                    }   
                    else
                    {
                        var allocDailyOutput = context.AllocDailyOutput.Where(x => x.LSStyle == lsStyle
                                                        && x.Size == cuttingOutput.Size
                                                        && x.FabricContrastName == cuttingOutput.FabricContrast).FirstOrDefault();
                        if(allocDailyOutput != null)
                        {
                            allocDailyOutput.PercentPrint += cuttingOutput.Quantity;
                            allocDailyOutput.SetUpdateAudit(userName);
                            context.AllocDailyOutput.Update(allocDailyOutput);

                            //create AllocTransaction
                            var allocTransaction = new AllocTransaction("CUTTING");
                            allocTransaction.CuttingOutputID = cuttingOutput.ID;
                            allocTransaction.AllocDailyOuputID = allocDailyOutput.ID;
                            allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                            allocTransaction.Size = allocDailyOutput.Size;
                            allocTransaction.Set = allocDailyOutput.Set;
                            allocTransaction.FabricContrastName = allocDailyOutput.FabricContrastName;
                            allocTransaction.AllocQuantity = cuttingOutput.Quantity;
                            allocTransaction.IsRetured = false;
                            allocTransaction.SetCreateAudit(userName);
                            context.AllocTransaction.Add(allocTransaction);
                            context.SaveChanges();
                        }
                    }    
                    //}    
                }    
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
        public List<string> getLSStyle(string mergeBlockLSStyle,string strLSStyle)
        {
            var result = new List<string>();

            var listLSStyle = strLSStyle.Split("+").ToList();
            foreach(var item in listLSStyle)
            {
                var groupLSStyle = item.Split("=>").ToList();
                if(groupLSStyle?.Count==1)
                {
                    result.Add(mergeBlockLSStyle + getIndexOfLsStyle(groupLSStyle[0]).ToString());
                }   
                else
                {
                    int startIndex = getIndexOfLsStyle(groupLSStyle[0]);
                    int endIndex = getIndexOfLsStyle(groupLSStyle[1]);
                    for(int i = startIndex; i <= endIndex; i++)
                    {
                        result.Add(mergeBlockLSStyle + i);
                    }    
                }    
            }    

            return result;
        }
        public int getIndexOfLsStyle(string lsStyle) //exple A1
        {
            int result = 0;
            int.TryParse(lsStyle.Substring(1, lsStyle.Length - 1),out result);
            return result;
        }
    }
   
    public class HALSStyle
    {
        public string  LSStyle { get; set; }
        public string Size { get; set; }
        public DateTime PSDD { get; set; }
        public string EstimatedPort { get; set; }
        public  int Index { get; set; }

    }
    public class DELSStyle
    {
        public string LSStyle { get; set; }
        public string Size { get; set; }
        public DateTime PSDD { get; set; }        
        public int Index { get; set; }

    }
    public class GALSStyle
    {
        public string LSStyle { get; set; }
        public string Size { get; set; }
        public string OrderTypeName { get; set; }
        public int Index { get; set; }
    }
}
