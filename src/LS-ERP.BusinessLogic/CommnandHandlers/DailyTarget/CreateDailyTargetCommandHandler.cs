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
    public class CreateDailyTargetCommandHandler
        : IRequestHandler<CreateDailyTargetCommand, CommonCommandResultHasData<DailyTarget>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateDailyTargetCommandHandler> logger;
        private readonly IMapper mapper;

        public CreateDailyTargetCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateDailyTargetCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<DailyTarget>> Handle(CreateDailyTargetCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<DailyTarget>();
            logger.LogInformation("{@time} - Exceute create job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute create job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            var dailyTarget = mapper.Map<DailyTarget>(request);
            if(dailyTarget.Operation=="SEWING")
            {
                //
                // Set CustomerName
                var customer = context.Customer.Where(x => x.ID == dailyTarget.CustomerID).FirstOrDefault();
                if (customer != null)
                {
                    dailyTarget.CustomerName = customer.Name;
                }
                // Set WorkCenterName
                var workCenter = context.WorkCenter.Where(x => x.ID == dailyTarget.WorkCenterID).FirstOrDefault();
                if (workCenter != null)
                {
                    dailyTarget.WorkCenterName = workCenter.Name;
                }                
                try
                {
                    dailyTarget.SetCreateAudit(request.UserName);
                    
                    context.DailyTarget.Add(dailyTarget);
                    context.SaveChanges();
                    //Create Joboutput
                    CreateJobOutput(dailyTarget);
                    //
                    result.Success = true;
                    result.SetData(dailyTarget);
                }
                catch (DbUpdateException ex)
                {
                    result.Message = ex.Message;
                    logger.LogInformation("{@time} - Exceute create job output command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                    LogHelper.Instance.Information("{@time} - Exceute create job output command with request {@request} has error {@error}",
                        DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                }
            }    
            else
            {
                try
                {
                    // Calculate totalTarget                       
                    dailyTarget.TotalTargetQuantity =
                        decimal.Round(dailyTarget.TargetQuantity
                                    + dailyTarget.Sample
                                    + dailyTarget.TargetQuantity * dailyTarget.Percent / 100, 0);
                    //
                    dailyTarget.SetCreateAudit(request.UserName);
                    context.DailyTarget.Add(dailyTarget);
                    context.SaveChanges();                 
                    result.Success = true;
                    result.SetData(dailyTarget);

                    var jobId = BackgroundJob.Enqueue<CreateAllocDailyOutputJob>(j => j.Execute(request.UserName));
                }
                catch (DbUpdateException ex)
                {
                    result.Message = ex.Message;
                    logger.LogInformation("{@time} - Exceute create job output command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                    LogHelper.Instance.Information("{@time} - Exceute create job output command with request {@request} has error {@error}",
                        DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                }
            }    

            return Task.FromResult(result);
        }
        public void CreateJobOutput(DailyTarget dailyTarget)
        {
            var workingTimes = context.WorkingTime.ToList();
            foreach(var workingTime in workingTimes)
            {
                var jobOutput = mapper.Map<JobOutput>(dailyTarget);

                // set OutputAt
                jobOutput.OutputAt = dailyTarget.ProduceDate;
                jobOutput.DailyTargetID = dailyTarget.ID;
                // set Target      
                jobOutput.TargetQuantity = decimal.Round(dailyTarget.TotalTargetQuantity / 10,0);
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
                //
                context.JobOuput.Add(jobOutput);
            }
            context.SaveChanges();
        }
    }
}
