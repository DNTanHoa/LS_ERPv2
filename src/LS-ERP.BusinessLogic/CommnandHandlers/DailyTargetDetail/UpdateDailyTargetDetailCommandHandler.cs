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
    public class UpdateDailyTargetDetailCommandHandler
        : IRequestHandler<UpdateDailyTargetDetailCommand, CommonCommandResultHasData<DailyTargetDetail>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateDailyTargetDetailCommandHandler> logger;
        private readonly IMapper mapper;

        public UpdateDailyTargetDetailCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateDailyTargetDetailCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<DailyTargetDetail>> Handle(UpdateDailyTargetDetailCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<DailyTargetDetail>();
            logger.LogInformation("{@time} - Exceute update job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute update job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var existDailyTargetDetail = context.DailyTargetDetail.FirstOrDefault(x => x.ID == request.ID);

                if(existDailyTargetDetail != null)
                {
                    
                    mapper.Map(request, existDailyTargetDetail);
                    existDailyTargetDetail.SetUpdateAudit(request.UserName);
                    //calculate efficiency
                    if(existDailyTargetDetail.Operation.Equals("SEWING"))
                    {
                        // Set CustomerName
                        var customer = context.Customer.Where(x => x.ID == existDailyTargetDetail.CustomerID).FirstOrDefault();
                        if (customer != null)
                        {
                            existDailyTargetDetail.CustomerName = customer.Name;
                        }
                        // Set WorkCenterName
                        var workCenter = context.WorkCenter.Where(x => x.ID == existDailyTargetDetail.WorkCenterID).FirstOrDefault();
                        if (workCenter != null)
                        {
                            existDailyTargetDetail.WorkCenterName = workCenter.Name;
                        }

                        // calculate efficiency
                       
                        if (existDailyTargetDetail.TotalTargetQuantity != 0)
                        {
                            if (existDailyTargetDetail.Quantity != null)
                            {
                                existDailyTargetDetail.Efficiency = 100 * existDailyTargetDetail.Quantity / existDailyTargetDetail.TotalTargetQuantity;
                            }
                        }
                        
                       
                        //
                        if (existDailyTargetDetail.IsAllocated)
                        {
                            existDailyTargetDetail.IsAllocated = false;
                        }
                        //
                    }
                    else
                    {
                        var w = context.WorkCenter.Where(x => x.ID == existDailyTargetDetail.WorkCenterID).FirstOrDefault();
                        if (w != null)
                        {
                            existDailyTargetDetail.WorkCenterName = w.Name;
                        }
                        if (existDailyTargetDetail.TotalOrderQuantity != null)
                        {
                            if (existDailyTargetDetail.TotalOrderQuantity != 0)
                            {
                                if (existDailyTargetDetail.Quantity != null)
                                {
                                    existDailyTargetDetail.Efficiency = 100 * existDailyTargetDetail.Quantity / existDailyTargetDetail.TotalOrderQuantity;
                                }
                            }
                        }
                    }

                    context.DailyTargetDetail.Update(existDailyTargetDetail);
                    context.SaveChanges();
                    result.Success = true;
                    result.SetData(existDailyTargetDetail);

                    ///
                    var jobId = BackgroundJob.Enqueue<AllocateDailyTargetDetailJob>(j=>j.Execute(request.UserName));

                    //Create Part Price
                    var jobPartPrice = BackgroundJob.Enqueue<CreatePartPriceJob>(j => j.Execute(existDailyTargetDetail));
                }
                else
                {
                    result.Message = "Can't not find to update";
                }
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute update job output command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute update job output command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
    }
}
