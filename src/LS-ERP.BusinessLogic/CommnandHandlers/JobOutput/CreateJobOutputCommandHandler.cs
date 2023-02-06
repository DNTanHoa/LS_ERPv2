using AutoMapper;
using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
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
    public class CreateJobOutputCommandHandler
        : IRequestHandler<CreateJobOutputCommand, CommonCommandResultHasData<JobOutput>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateJobOutputCommandHandler> logger;
        private readonly IMapper mapper;

        public CreateJobOutputCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateJobOutputCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<JobOutput>> Handle(CreateJobOutputCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<JobOutput>();
            logger.LogInformation("{@time} - Exceute create job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute create job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            var jobOutput = mapper.Map<JobOutput>(request);
            // set OutputAt
            jobOutput.OutputAt = DateTime.Now;
            // Calculate Efficiency            
            if (jobOutput.TargetQuantity!=null)
            {
                if(jobOutput.TargetQuantity!=0)
                {
                    if(jobOutput.Quantity!=null)
                    {
                        jobOutput.Efficiency = jobOutput.Quantity / jobOutput.TargetQuantity;
                    }
                    else
                    {
                        jobOutput.Efficiency = 0;
                    }
                }
                else
                {
                    jobOutput.Efficiency = 0;
                }
            }
            else
            {
                jobOutput.Efficiency = 0;
            }
            // Set WorkCenterName
            var customer = context.Customer.Where(x => x.ID == jobOutput.CustomerID).FirstOrDefault();
            if (customer != null)
            {
                jobOutput.CustomerName = customer.Name;
            }
            // Set WorkCenterName
            var workCenter = context.WorkCenter.Where(x => x.ID == jobOutput.WorkCenterID).FirstOrDefault();
            if (workCenter != null)
            {
                jobOutput.WorkCenterName = workCenter.Name;
                jobOutput.DepartmentID = workCenter.DepartmentID;
            }
            //
            // Set WorkingTimeName
            var workingTime = context.WorkingTime.Where(x => x.ID == jobOutput.WorkingTimeID).FirstOrDefault();
            if(workingTime!=null)
            {
                jobOutput.WorkingTimeName = workingTime.Name;
            }
            //
            // Set ItemStyleDescription
            var customerStyle = context.ViewCustomerStyle
                                                        .Select(x => new CustomerItemStyleDtos
                                                        {
                                                            CustomerStyle = x.CustomerStyle,
                                                            CustomerId = x.CustomerId,
                                                            CustomerName = x.CustomerName,
                                                            ItemStyleDescription = x.ItemStyleDescription
                                                        })
                                                        .Where(x => x.CustomerId == jobOutput.CustomerID &&
                                                                x.CustomerStyle == jobOutput.StyleNO)
                                                        .FirstOrDefault();
            if (customerStyle !=null)
            {
                jobOutput.ItemStyleDescription = customerStyle.ItemStyleDescription;
            }    
            //

            try
            {
                jobOutput.SetCreateAudit(request.UserName);
                context.JobOuput.Add(jobOutput);
                context.SaveChanges();

                result.Success = true;
                result.SetData(jobOutput);
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
