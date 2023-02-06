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
    public class BulkDailyTargetCommandHandler : IRequestHandler<BulkDailyTargetCommand,
        CommonCommandResultHasData<IEnumerable<DailyTarget>>>
    {
        private readonly ILogger<BulkDailyTargetCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public BulkDailyTargetCommandHandler(ILogger<BulkDailyTargetCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<IEnumerable<DailyTarget>>> Handle(
            BulkDailyTargetCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<IEnumerable<DailyTarget>>();
            logger.LogInformation("{@time} - Exceute bulk job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute bulk job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());    
            
            if(request.Data != null)
            {
                if(request.Data.Count > 0)
                {
                    request.UserName = request.Data.FirstOrDefault().UserName;
                }
            }
            try
            {
                var dailyTargets = request.Data
                    .Select(x => mapper.Map<DailyTarget>(x)).ToList();
                
                foreach(var dailyTarget in dailyTargets)
                {
                    if(dailyTarget.Operation=="SEWING")
                    {
                        // Set CustomerID
                        var customer = context.Customer.Where(x => x.Name == dailyTarget.CustomerName).FirstOrDefault();
                        if (customer != null)
                        {
                            dailyTarget.CustomerID = customer.ID;
                        }
                        // Set WorkCenterID
                        var workCenter = context.WorkCenter.Where(x => x.Name == dailyTarget.WorkCenterName).FirstOrDefault();
                        if (workCenter != null)
                        {
                            dailyTarget.WorkCenterID = workCenter.ID;
                        }

                        //remove dailyTarget if exist
                        var fromProduceDate = dailyTarget.ProduceDate;
                        var toProduceDate = fromProduceDate.AddDays(1);
                        var existDailyTarget = context.DailyTarget.Where(x => x.WorkCenterID == dailyTarget.WorkCenterID
                                              && (x.ProduceDate.Date >= fromProduceDate.Date && x.ProduceDate < toProduceDate.Date)
                                                                            ).FirstOrDefault();
                        if (existDailyTarget != null)
                        {
                            //remove JobOutput with DailyTargetID
                            var jobOutput = context.JobOuput.Where(x => x.DailyTargetID == existDailyTarget.ID).ToList();
                            if (jobOutput != null)
                            {
                                foreach (var job in jobOutput)
                                {
                                    context.JobOuput.Remove(job);
                                }
                            }
                            //remove DailyTargetDetail with DailyTargetID
                            var dailyTargetDetails = context.DailyTargetDetail.Where(x => x.DailyTargetID == existDailyTarget.ID).ToList();
                            if (dailyTargetDetails != null)
                            {
                                foreach (var dailyTargetDetail in dailyTargetDetails)
                                {
                                    context.DailyTargetDetail.Remove(dailyTargetDetail);
                                }
                            }
                            //
                            context.DailyTarget.Remove(existDailyTarget);
                        }
                        //
                        dailyTarget.SetCreateAudit(request.UserName);
                        context.DailyTarget.Add(dailyTarget);
                        context.SaveChanges();
                        //Create Joboutput
                        CreateJobOutput(dailyTarget, request.UserName);
                        //Create DailyTargetDetail
                        CreateDailyTargetDetail(dailyTarget, request.UserName);
                        //Job Create Part Price
                        var jobPartPrice = BackgroundJob.Enqueue<CreatePartPriceJob>(j => j.Execute(dailyTarget));

                    }
                    else if(dailyTarget.Operation=="CUTTING")
                    {
                        // remove dailyTarget if exist
                        var existDailyTarget = context.DailyTarget.Where(x => x.Operation == "CUTTING"                                                                                                                                                     
                                                                            && x.LSStyle == dailyTarget.LSStyle
                                                                            && x.CompanyID == dailyTarget.CompanyID
                                                                            && x.IsDeleted == false
                                                                            ).FirstOrDefault();
                        if (existDailyTarget == null)
                        {
                            // Calculate totalTarget                       
                            dailyTarget.TotalTargetQuantity =
                                decimal.Round(dailyTarget.TargetQuantity + dailyTarget.Sample + dailyTarget.TargetQuantity * dailyTarget.Percent / 100, 0);
                            // Set CustomerID, customerName
                            var itemStyle = context.ItemStyle.Where(x => x.LSStyle == dailyTarget.LSStyle).FirstOrDefault();
                            if (itemStyle != null)
                            {
                                var salesOrder = context.SalesOrders.Where(x => x.ID == itemStyle.SalesOrderID).FirstOrDefault();
                                if (salesOrder != null)
                                {
                                    dailyTarget.CustomerID = salesOrder.CustomerID;
                                    dailyTarget.CustomerName = salesOrder.CustomerName;
                                }
                            }
                            // Set ETAPORT
                            dailyTarget.EstimatedPort = context.ItemStyle.Where(x => x.LSStyle == dailyTarget.LSStyle
                                                                    && x.SalesOrderID.Contains("HADDAD")
                                                                    ).Select(x => x.ETAPort).FirstOrDefault();


                            //Set OrderType
                            dailyTarget.OrderTypeName = context.ItemStyle.Where(x => x.LSStyle == dailyTarget.LSStyle)
                                .Select(x => x.ExternalPurchaseOrderTypeName).FirstOrDefault();
                            /////
                            ///Set Season
                            dailyTarget.Season = dailyTarget.CustomerID.Equals("HA") ?
                                context.ItemStyle.Where(x => x.LSStyle == dailyTarget.LSStyle
                                    && x.SalesOrderID.Contains("HADDAD")).Select(x => x.Season).FirstOrDefault() +
                                context.ItemStyle.Where(x => x.LSStyle == dailyTarget.LSStyle
                                    && x.SalesOrderID.Contains("HADDAD")).Select(x => x.Year).FirstOrDefault().ToString().Substring(2)
                              : context.ItemStyle.Where(x => x.LSStyle == dailyTarget.LSStyle
                                    && x.SalesOrderID.Contains("HADDAD")).Select(x => x.Season).FirstOrDefault();
                            ///
                            dailyTarget.SetCreateAudit(request.UserName);
                            context.DailyTarget.Add(dailyTarget);
                            context.SaveChanges();
                        }
                        else
                        {
                            //context.DailyTarget.Remove(existDailyTarget);
                            ////////remove AllocDailyOutput with TargetID
                            //var allocDailyOutputs = context.AllocDailyOutput.Where(x => x.TargetID == existDailyTarget.ID).ToList();
                            //if (allocDailyOutputs != null)
                            //{
                            //    foreach (var allocDailyOutput in allocDailyOutputs)
                            //    {
                            //        context.AllocDailyOutput.Remove(allocDailyOutput);
                            //    }
                            //}
                        }    
                        
                    }  
                }
                //Call job create detail alloc cutting by size, on
                var jobId = BackgroundJob.Enqueue<CreateAllocDailyOutputJob>(j => j.Execute(request.UserName));
                //
                result.Success = true;
                result.Data = dailyTargets;
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute bulk dailyTarget command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute bulk dailyTarget command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
        public void CreateJobOutput(DailyTarget dailyTarget,string userName)
        {
            var workingTimes = context.WorkingTime.ToList();
            foreach (var workingTime in workingTimes)
            {
                var jobOutput = mapper.Map<JobOutput>(dailyTarget);

                // set OutputAt
                jobOutput.OutputAt = dailyTarget.ProduceDate;
                jobOutput.DailyTargetID = dailyTarget.ID;
                // set Target
                jobOutput.TotalTargetQuantity = dailyTarget.TotalTargetQuantity;
                jobOutput.TargetQuantity = decimal.Round(dailyTarget.TotalTargetQuantity / 10, 0);
                // Set CustomerName
                jobOutput.CustomerName = dailyTarget.CustomerName;
                // Set WorkCenterName
                var workCenter = context.WorkCenter.Where(x => x.ID == dailyTarget.WorkCenterID).FirstOrDefault();
                if (workCenter != null)
                {
                    jobOutput.WorkCenterName = workCenter.Name;
                    jobOutput.DepartmentID = workCenter.DepartmentID;
                }
                // Set WorkingTimeName              
                jobOutput.WorkingTimeName = workingTime.Name;
                jobOutput.WorkingTimeID = workingTime.ID;
                // Set ItemStyleDescription
                var customerStyle = context.ViewCustomerStyle
                                                            .Select(x => new CustomerItemStyleDtos
                                                            {
                                                                CustomerStyle = x.CustomerStyle,
                                                                CustomerId = x.CustomerId,
                                                                CustomerName = x.CustomerName,
                                                                ItemStyleDescription = x.ItemStyleDescription
                                                            })
                                                            .Where(x => x.CustomerId == dailyTarget.CustomerID &&
                                                                    x.CustomerStyle == dailyTarget.StyleNO)
                                                            .FirstOrDefault();
                if (customerStyle != null)
                {
                    jobOutput.ItemStyleDescription = customerStyle.ItemStyleDescription;
                }
                // Set 
                jobOutput.Operation = dailyTarget.Operation;
                jobOutput.SetCreateAudit(userName);
                //
                context.JobOuput.Add(jobOutput);
            }
            context.SaveChanges();
        }

        public void CreateDailyTargetDetail (DailyTarget dailyTarget,string userName)
        {
            var dailyTargetDetail = mapper.Map<DailyTargetDetail>(dailyTarget);
            dailyTargetDetail.DailyTargetID = dailyTarget.ID;
            dailyTargetDetail.SetCreateAudit(userName);
            context.DailyTargetDetail.Add(dailyTargetDetail);
            context.SaveChanges();

        }
        
    }
}
