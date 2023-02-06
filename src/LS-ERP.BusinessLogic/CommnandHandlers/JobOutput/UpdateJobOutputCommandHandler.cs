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
    public class UpdateJobOutputCommandHandler
        : IRequestHandler<UpdateJobOutPutCommand, CommonCommandResultHasData<JobOutput>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateJobOutputCommandHandler> logger;
        private readonly IMapper mapper;

        public UpdateJobOutputCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateJobOutputCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<JobOutput>> Handle(UpdateJobOutPutCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<JobOutput>();
            logger.LogInformation("{@time} - Exceute update job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute update job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var existJobOutput = context.JobOuput.FirstOrDefault(x => x.ID == request.ID);
               
                if(existJobOutput != null)
                {
                    
                    //check SMV of sylteNO changed
                    bool isSMVChanged = false;
                    var dailyTarget = context.DailyTarget.FirstOrDefault(x => x.ID == existJobOutput.DailyTargetID);
                    if (request.SMV != dailyTarget.SMV && dailyTarget.StyleNO == request.StyleNO)
                    {
                        isSMVChanged = true;
                    }
                    if (dailyTarget.StyleNO != request.StyleNO)
                    {
                        dailyTarget.StyleNO = request.StyleNO;
                        dailyTarget.SMV = request.SMV;
                        if (dailyTarget.SMV != null)
                        {
                            if (dailyTarget.SMV != 0)
                            {
                                dailyTarget.TotalTargetQuantity = decimal.Round((decimal)(existJobOutput.NumberOfWorker * 660) / (decimal)dailyTarget.SMV, 0);
                            }
                        }
                        else
                        {
                            //dailyTarget.TotalTargetQuantity = 0;
                        }
                        dailyTarget.SetUpdateAudit(request.UserName);
                        context.DailyTarget.Update(dailyTarget);
                        context.SaveChanges();
                    }    
                    
                    //
                    mapper.Map(request, existJobOutput);
                    existJobOutput.SetUpdateAudit(request.UserName);
                    // Calculate Efficiency                    
                    if (existJobOutput.TargetQuantity != null)
                    {
                        if (existJobOutput.TargetQuantity != 0)
                        {
                            if (existJobOutput.Quantity != null)
                            {
                                //existJobOutput.Efficiency = 100*existJobOutput.Quantity / existJobOutput.TargetQuantity;
                                existJobOutput.Efficiency = decimal.Round((decimal)(100 * existJobOutput.Quantity / existJobOutput.TargetQuantity), 2);
                            }
                            else
                            {
                                existJobOutput.Efficiency = 0;
                            }    
                        }
                        else
                        {
                            existJobOutput.Efficiency = 0;
                        }
                    }
                    else
                    {
                        existJobOutput.Efficiency = 0;
                    }
                    // Update CustomerName
                    var customer = context.Customer.Where(x => x.ID == existJobOutput.CustomerID).FirstOrDefault();
                    if (customer != null)
                    {
                        existJobOutput.CustomerName = customer.Name;
                    }
                    // Update WorkingTimeName
                    var workingTime = context.WorkingTime.Where(x => x.ID == existJobOutput.WorkingTimeID).FirstOrDefault();
                    if (workingTime != null)
                    {
                        existJobOutput.WorkingTimeName = workingTime.Name;
                    }
                    //
                    // Update ItemStyleDescription
                    var customerStyle = context.ViewCustomerStyle
                                                        .Select(x => new CustomerItemStyleDtos
                                                        {
                                                            CustomerStyle = x.CustomerStyle,
                                                            CustomerId = x.CustomerId,
                                                            CustomerName = x.CustomerName,
                                                            ItemStyleDescription = x.ItemStyleDescription
                                                        })
                                                        .Where(x => x.CustomerId == existJobOutput.CustomerID &&
                                                                x.CustomerStyle == existJobOutput.StyleNO)
                                                        .FirstOrDefault();
                    if (customerStyle != null)
                    {
                        existJobOutput.ItemStyleDescription = customerStyle.ItemStyleDescription;
                    }
                    //Check and update next JobOutPut workingTime if current JobOutput change style
                    var listNextJobOuput = context.JobOuput.Where(x => x.DailyTargetID == existJobOutput.DailyTargetID
                                                                    && x.WorkingTimeID > existJobOutput.WorkingTimeID
                                                                    && x.Quantity == null).ToList();
                    if (listNextJobOuput != null)
                    {
                        foreach(var item in listNextJobOuput)
                        {
                            var existNextJobOutput = context.JobOuput.FirstOrDefault(x => x.ID == item.ID);
                            if(existNextJobOutput != null)
                            {
                                existNextJobOutput.TargetQuantity = existJobOutput.TargetQuantity;
                                existNextJobOutput.TotalTargetQuantity = existJobOutput.TotalTargetQuantity;
                                existNextJobOutput.StyleNO = existJobOutput.StyleNO;
                                existNextJobOutput.ItemStyleDescription = existJobOutput.ItemStyleDescription;
                                existNextJobOutput.CustomerID = existJobOutput.CustomerID;
                                existNextJobOutput.CustomerName = existJobOutput.CustomerName;
                                existNextJobOutput.NumberOfWorker = existJobOutput.NumberOfWorker;
                                context.JobOuput.Update(existNextJobOutput);
                                context.SaveChanges();
                            }    
                        }    
                    }
                    //
                    // Check and update prev JobOutput workingTime if quantity is null
                    var listPrevJobOut = context.JobOuput.Where(x => x.DailyTargetID == existJobOutput.DailyTargetID
                                                                && x.WorkingTimeID < existJobOutput.WorkingTimeID
                                                                && x.Quantity == null
                                                                ).ToList();
                    if (listPrevJobOut != null)
                    {
                        foreach (var item in listPrevJobOut)
                        {
                            var existPrevJobOutput = context.JobOuput.FirstOrDefault(x => x.ID == item.ID);
                            if (existPrevJobOutput != null)
                            {
                                existPrevJobOutput.TargetQuantity = existJobOutput.TargetQuantity;
                                existPrevJobOutput.StyleNO = existJobOutput.StyleNO;
                                existPrevJobOutput.ItemStyleDescription = existJobOutput.ItemStyleDescription;
                                existPrevJobOutput.CustomerID = existJobOutput.CustomerID;
                                existPrevJobOutput.CustomerName = existJobOutput.CustomerName;
                                existPrevJobOutput.NumberOfWorker = existJobOutput.NumberOfWorker;
                                existPrevJobOutput.Quantity = existJobOutput.Quantity;
                                existPrevJobOutput.Efficiency = existJobOutput.Efficiency;
                                existPrevJobOutput.SetUpdateAudit(request.UserName);
                                context.JobOuput.Update(existPrevJobOutput);
                                context.SaveChanges();
                            }
                        }
                    }
                    // update smv of sylteNO and cal eff of JobOutput
                    if(isSMVChanged)
                    {
                        dailyTarget.SMV = request.SMV;
                        if(dailyTarget.SMV != null)
                        {
                            if(dailyTarget.SMV !=0)
                            {
                                dailyTarget.TotalTargetQuantity = decimal.Round((decimal)(existJobOutput.NumberOfWorker * 660) / (decimal)dailyTarget.SMV,0);
                            }    
                        }
                        dailyTarget.SetUpdateAudit(request.UserName);
                        context.Update(dailyTarget);
                        context.SaveChanges();
                        var jobOutputSMV = context.JobOuput.Where(j => j.DailyTargetID == dailyTarget.ID
                                                                    && j.StyleNO == dailyTarget.StyleNO).ToList();
                        foreach(var job in jobOutputSMV)
                        {
                            
                            if (job.Quantity != null)
                            {
                                //existJobOutput.Efficiency = 100*existJobOutput.Quantity / existJobOutput.TargetQuantity;
                                job.TotalTargetQuantity = dailyTarget.TotalTargetQuantity;
                                job.TargetQuantity = decimal.Round(dailyTarget.TotalTargetQuantity / 10,0);
                                job.Efficiency = decimal.Round((decimal)(100 * job.Quantity / job.TargetQuantity), 2);
                                        
                            }
                            else
                            {
                                job.Efficiency = null;
                            }
                             
                            job.SetUpdateAudit(request.UserName);
                            context.JobOuput.Update(job);
                            context.SaveChanges();
                        }    
                    }    

                    //
                    context.JobOuput.Update(existJobOutput);
                    context.SaveChanges();

                    result.Success = true;
                    result.SetData(existJobOutput);
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
