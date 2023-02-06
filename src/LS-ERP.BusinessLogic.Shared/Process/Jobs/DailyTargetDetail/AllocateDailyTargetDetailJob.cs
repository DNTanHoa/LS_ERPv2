using Hangfire;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class AllocateDailyTargetDetailJob
    {
        private readonly ILogger<AllocateDailyTargetDetailJob> logger;
        private readonly SqlServerAppDbContext context;

        public AllocateDailyTargetDetailJob(ILogger<AllocateDailyTargetDetailJob> logger,
            SqlServerAppDbContext context) 
        {
            this.logger = logger;
            this.context = context;
        }

        [JobDisplayName("Allocate Output Quantity with size orderdetail")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute(string userName)
        {
            /// Update handle
            try
            {
                var dailyTargetDetails = context.DailyTargetDetail.Where(x => x.IsAllocated == false
                                                                        && x.Operation =="SEWING"
                                                                        ).ToList();
                foreach(var dailyTargetDetail in dailyTargetDetails)
                {
                    //Insert AllocDailyOutput if not exist
                    if(!string.IsNullOrEmpty(dailyTargetDetail.LSStyle))
                    {
                        var lsStyles = dailyTargetDetail.LSStyle.Split(",");
                        foreach(var lsStyle in lsStyles)
                        {
                            InsertAllocDailyOutput(lsStyle,userName);                            
                        }    
                    }
                    //
                    //reverse Quantity if exist
                    ReturnAllocDailyOutput(dailyTargetDetail,userName);

                    // Update AllocDailyOutput
                    if(dailyTargetDetail.Quantity != null)
                    {
                        decimal allocQuantity = (decimal)dailyTargetDetail.Quantity;
                        if (!string.IsNullOrEmpty(dailyTargetDetail.LSStyle))
                        {
                            var lsStyles = dailyTargetDetail.LSStyle.Split(",");
                            foreach (var lsStyle in lsStyles)
                            {
                                if (allocQuantity > 0)
                                {
                                    allocQuantity = UpdateAllocDailyOutput(lsStyle, dailyTargetDetail.ID, allocQuantity,userName);
                                }
                            }
                        }
                        if(allocQuantity ==0)
                        {
                            dailyTargetDetail.IsAllocated = true;
                        }    
                    }                    
                }
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("Allocate Output Quantity with size orderdetail event handler has error with message {@message}",
                    ex.InnerException?.Message);
                LogHelper.Instance.Error("Allocate Output Quantity with size orderdetail event handler has error with message {@message}",
                    ex.InnerException?.Message);
            }

            return Task.CompletedTask;  
        }

        public void ReturnAllocDailyOutput(DailyTargetDetail dailyTargetDetail,string userName)
        {
       
            var allocTransactions = context.AllocTransaction.Where(x=>  x.DailyTargetDetailID == dailyTargetDetail.ID
                                                                    && x.IsRetured == false
                                                                ).ToList();
            if(allocTransactions!=null)
            {
                foreach(var allocTransaction in allocTransactions )
                {
                    var allocDailyOutput = context.AllocDailyOutput.Where(x => x.LSStyle == allocTransaction.LSStyle
                                                                         && x.Size == allocTransaction.Size
                                                                         && x.Operation == "SEWING"
                                                                         ).FirstOrDefault();
                    if(allocDailyOutput != null)
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
        public void InsertAllocDailyOutput(string lsStyle,string userName)
        {
            var orderDetails = context.OrderDetail
                   .Where(o => o.ItemStyle.LSStyle.Equals(lsStyle.Trim())).ToList();
            if (orderDetails != null)
            {
                if (orderDetails.Count > 0)
                {
                    foreach (var orderDetail in orderDetails)
                    {
                        var existAllocDailyOutput = context.AllocDailyOutput.Where(a=>a.LSStyle.Equals(lsStyle)
                                                                                && a.Size.Equals(orderDetail.Size)
                                                                                && a.Operation == "SEWING"
                                                                                ).FirstOrDefault();
                        if(existAllocDailyOutput == null)
                        {
                            var allocDailyOutput = new AllocDailyOutput("SEWING");
                            allocDailyOutput.LSStyle = lsStyle;
                            allocDailyOutput.Size = orderDetail.Size;
                            allocDailyOutput.OrderQuantity = (decimal)orderDetail.Quantity;
                            allocDailyOutput.Quantity = 0;
                            allocDailyOutput.IsFull = false;                            
                            allocDailyOutput.SetCreateAudit(userName);
                            context.AllocDailyOutput.Add(allocDailyOutput);
                        }    
                    }
                    context.SaveChanges();
                }
            }
        }
        public decimal UpdateAllocDailyOutput(string lsStyle, int dailyTargetDetailID,decimal totalQuantity,string userName)
        {
            var curQuantity = totalQuantity;
            var allocDailyOutputs = context.AllocDailyOutput.Where(x => x.LSStyle == lsStyle).ToList();
            if(allocDailyOutputs!=null)
            {
                foreach(var allocDailyOutput in allocDailyOutputs)
                {
                    var issueQuantity = allocDailyOutput.OrderQuantity - allocDailyOutput.Quantity;
                    if(issueQuantity <= curQuantity)
                    {
                        allocDailyOutput.Quantity = allocDailyOutput.Quantity + issueQuantity;
                        allocDailyOutput.IsFull = true;
                        allocDailyOutput.SetUpdateAudit(userName);
                        curQuantity = curQuantity - issueQuantity;
                        var allocTransaction = new AllocTransaction("SEWING");
                        allocTransaction.DailyTargetDetailID = dailyTargetDetailID;
                        allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                        allocTransaction.Size = allocDailyOutput.Size;
                        allocTransaction.AllocQuantity = issueQuantity;
                        allocTransaction.IsRetured = false;
                        allocTransaction.SetCreateAudit(userName);
                        context.AllocTransaction.Add(allocTransaction);
                    }
                    else 
                    {
                        issueQuantity = curQuantity;
                        curQuantity = 0;
                        allocDailyOutput.Quantity = allocDailyOutput.Quantity + issueQuantity;
                        allocDailyOutput.SetUpdateAudit(userName);
                        var allocTransaction = new AllocTransaction("SEWING");
                        allocTransaction.DailyTargetDetailID = dailyTargetDetailID;
                        allocTransaction.LSStyle = allocDailyOutput.LSStyle;
                        allocTransaction.Size = allocDailyOutput.Size;
                        allocTransaction.AllocQuantity = issueQuantity;
                        allocTransaction.IsRetured = false;
                        allocTransaction.SetCreateAudit(userName);
                        context.AllocTransaction.Add(allocTransaction);
                        context.SaveChanges();
                        return curQuantity;
                    }
                }
                context.SaveChanges();
            }
            return curQuantity;
        }
    }
}
