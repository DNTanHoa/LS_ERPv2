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
    public class CreateDailyTargetDetailCommandHandler
        : IRequestHandler<CreateDailyTargetDetailCommand, CommonCommandResultHasData<DailyTargetDetail>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateDailyTargetDetailCommandHandler> logger;
        private readonly IMapper mapper;

        public CreateDailyTargetDetailCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateDailyTargetDetailCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<DailyTargetDetail>> Handle(CreateDailyTargetDetailCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<DailyTargetDetail>();
            logger.LogInformation("{@time} - Exceute create job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute create job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            var dailyTargetDetail = mapper.Map<DailyTargetDetail>(request);

            if (dailyTargetDetail.Operation == "CUTTING")
            {
                dailyTargetDetail.IsAllocated = true;
                var w = context.WorkCenter.Where(x => x.ID == dailyTargetDetail.WorkCenterID).FirstOrDefault();
                if (w != null)
                {
                    dailyTargetDetail.WorkCenterName = w.Name;
                }
                // calculate efficiency
                if (dailyTargetDetail.TotalOrderQuantity != null)
                {
                    if (dailyTargetDetail.TotalOrderQuantity != 0)
                    {
                        if (dailyTargetDetail.Quantity != null)
                        {
                            dailyTargetDetail.Efficiency = 100 * dailyTargetDetail.Quantity / dailyTargetDetail.TotalOrderQuantity;
                        }
                    }
                }
            }
            else if (dailyTargetDetail.Operation == "SEWING")
            {
                //
                if(dailyTargetDetail.DailyTargetID ==0)
                {
                    var dailyTarget = context.DailyTarget.Where(x => x.ProduceDate.Date == dailyTargetDetail.ProduceDate.Date
                                                                && x.ProduceDate.Month == dailyTargetDetail.ProduceDate.Month
                                                                && x.ProduceDate.Year == dailyTargetDetail.ProduceDate.Year
                                                                && x.Operation == "SEWING"
                                                                && x.WorkCenterID == dailyTargetDetail.WorkCenterID
                                                                ).FirstOrDefault();
                    dailyTargetDetail.DailyTargetID = dailyTarget.ID;
                }    
                // Set CustomerName
                var customer = context.Customer.Where(x => x.ID == dailyTargetDetail.CustomerID).FirstOrDefault();
                if (customer != null)
                {
                    dailyTargetDetail.CustomerName = customer.Name;
                }
                // Set WorkCenterName
                var workCenter = context.WorkCenter.Where(x => x.ID == dailyTargetDetail.WorkCenterID).FirstOrDefault();
                if (workCenter != null)
                {
                    dailyTargetDetail.WorkCenterName = workCenter.Name;
                }

                // calculate efficiency
                if (dailyTargetDetail.TotalTargetQuantity != null)
                {
                    if (dailyTargetDetail.TotalTargetQuantity != 0)
                    {
                        if (dailyTargetDetail.Quantity != null)
                        {
                            dailyTargetDetail.Efficiency = 100 * dailyTargetDetail.Quantity / dailyTargetDetail.TotalTargetQuantity;
                        }
                    }
                }
            }
            try
            {
                dailyTargetDetail.SetCreateAudit(request.UserName);
                
                context.DailyTargetDetail.Add(dailyTargetDetail);
                context.SaveChanges();

                //Create Part Price
                var jobPartPrice = BackgroundJob.Enqueue<CreatePartPriceJob>(j => j.Execute(dailyTargetDetail));

                result.Success = true;
                result.SetData(dailyTargetDetail);
                if(dailyTargetDetail.Operation == "SEWING" && dailyTargetDetail.Quantity != null)
                {
                    var jobId = BackgroundJob.Enqueue<AllocateDailyTargetDetailJob>(j => j.Execute(request.UserName));
                }    
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute create job output command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute create job output command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
       
    }
}
