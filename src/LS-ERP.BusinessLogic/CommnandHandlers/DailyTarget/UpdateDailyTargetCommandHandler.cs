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
    public class UpdateDailyTargetCommandHandler
        : IRequestHandler<UpdateDailyTargetCommand, CommonCommandResultHasData<DailyTarget>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateDailyTargetCommandHandler> logger;
        private readonly IMapper mapper;

        public UpdateDailyTargetCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateDailyTargetCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<DailyTarget>> Handle(UpdateDailyTargetCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<DailyTarget>();
            logger.LogInformation("{@time} - Exceute update job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute update job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var existDailyTarget = context.DailyTarget.FirstOrDefault(x => x.ID == request.ID);
                
               
                if(existDailyTarget != null)
                {
                    bool IsChangedFabricContrast = false;
                    if(existDailyTarget.FabricContrast != null)
                    {
                        if (!existDailyTarget.FabricContrast.Equals(request.FabricContrast))
                        {
                            IsChangedFabricContrast = true;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.FabricContrast))
                        {
                            IsChangedFabricContrast = true;
                        }
                    }
                    mapper.Map(request, existDailyTarget);
                    // Calculate totalTarget                       
                    existDailyTarget.TotalTargetQuantity =
                        decimal.Round(existDailyTarget.TargetQuantity 
                                    + existDailyTarget.Sample 
                                    + existDailyTarget.TargetQuantity * existDailyTarget.Percent / 100, 0);
                    //

                    existDailyTarget.SetUpdateAudit(request.UserName);
                    existDailyTarget.IsCreatedDetail = true;
                    context.DailyTarget.Update(existDailyTarget);
                    context.SaveChanges();
                    //
                    if (existDailyTarget.Operation == "CUTTING")
                    {
                        var allocDailyOutputs = context.AllocDailyOutput.Where(x=>x.TargetID == existDailyTarget.ID).ToList();
                        foreach(var allocDailyOutput in allocDailyOutputs)
                        {
                            allocDailyOutput.IsCanceled = existDailyTarget.IsCanceled;
                            allocDailyOutput.SetUpdateAudit(request.UserName);
                            context.AllocDailyOutput.Update(allocDailyOutput);
                        }
                        if(IsChangedFabricContrast)
                        {
                            CreateAllocDailyOutput(existDailyTarget, request.UserName);
                            existDailyTarget.IsCreatedDetail = true;
                            context.DailyTarget.Update(existDailyTarget);
                        }    
                    }    
                    context.SaveChanges();

                    result.Success = true;
                    result.SetData(existDailyTarget);
                    var jobId = BackgroundJob.Enqueue<CreateAllocDailyOutputJob>(j => j.Execute(request.UserName));
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
        public void CreateAllocDailyOutput(DailyTarget dailyTarget,string userName)
        {
            CreateListFabricContrast(dailyTarget, userName);
            var Sets = dailyTarget.Set.Split(";").ToList();
            var sampleSizes = new List<SampleSize>();
            if (!string.IsNullOrEmpty(dailyTarget.SampleWithSize))
            {
                var listSample = dailyTarget.SampleWithSize.Split(";").ToList();
                foreach (var sample in listSample)
                {
                    var s = sample.Split("#").ToList();
                    if (s.Count >= 2)
                    {
                        var sampleSize = new SampleSize();
                        sampleSize.Size = s[0];
                        decimal dec = 0;
                        decimal.TryParse(s[1], out dec);
                        sampleSize.SampleQuantity = dec;
                        sampleSizes.Add(sampleSize);
                    }
                }
            }
            
            //
            foreach (var set in Sets)
            {
                var orderDetails = context.OrderDetail.Where(x => x.ItemStyle.LSStyle == dailyTarget.LSStyle
                                                            && x.ItemStyle.TotalQuantity != 0
                                                            && (
                                                            (x.ItemStyle.SalesOrderID.Contains("HADDAD") && x.ItemStyle.SalesOrder.CustomerID.Equals("HA")
                                                            || !x.ItemStyle.SalesOrder.CustomerID.Equals("HA"))
                                                            )).ToList();

                foreach (var orderDetail in orderDetails)
                {
                    var fabricContrasts = context.FabricContrast.Where(x => x.MergeBlockLSStyle == dailyTarget.MergeBlockLSStyle).ToList();
                    var sampleSize = sampleSizes.Where(x => x.Size == orderDetail.Size).FirstOrDefault();
                    foreach (var fabricContrast in fabricContrasts)
                    {
                        var existAllocDailyOutputs = context.AllocDailyOutput.Where(x => x.TargetID == dailyTarget.ID
                                                                        && x.LSStyle == dailyTarget.LSStyle
                                                                        && x.Size == orderDetail.Size
                                                                        && x.FabricContrastName == fabricContrast.Name
                                                                        ).ToList();
                        if (existAllocDailyOutputs.Count()==0)
                        {
                            var allocDailyOutput = mapper.Map<AllocDailyOutput>(dailyTarget);
                            allocDailyOutput.TargetID = dailyTarget.ID;
                            allocDailyOutput.Size = orderDetail.Size.Trim().ToUpper();
                            allocDailyOutput.Set = set;
                            if(orderDetail.ConsumedQuantity !=null)
                            {
                                allocDailyOutput.OrderQuantity = (decimal)(orderDetail.Quantity - orderDetail.ConsumedQuantity!);
                                allocDailyOutput.PercentQuantity = decimal.Round((decimal)(orderDetail.Quantity - orderDetail.ConsumedQuantity!) * allocDailyOutput.Percent / 100, 0);
                            }   
                            else
                            {
                                allocDailyOutput.OrderQuantity = (decimal)(orderDetail.Quantity);
                                allocDailyOutput.PercentQuantity = decimal.Round((decimal)(orderDetail.Quantity) * allocDailyOutput.Percent / 100, 0);
                            }    
                            
                            allocDailyOutput.Sample = sampleSize == null ? dailyTarget.Sample : sampleSize.SampleQuantity;
                            allocDailyOutput.FabricContrastID = fabricContrast.ID;
                            allocDailyOutput.FabricContrastName = fabricContrast.Name;
                            allocDailyOutput.SetCreateAudit(userName);
                            context.AllocDailyOutput.Add(allocDailyOutput);
                            context.SaveChanges();
                        }    
                        
                    }
                }
            }
            
        }
        public void CreateListFabricContrast(DailyTarget dailyTarget, string userName)
        {
            var colorCode = string.Empty;
            colorCode = context.ItemStyle.Where(x => x.LSStyle == dailyTarget.LSStyle).Select(x => x.ColorCode).FirstOrDefault();
            if (!string.IsNullOrEmpty(dailyTarget.FabricContrast))
            {


                var fabricContrasts = dailyTarget.FabricContrast.Split(";");
                foreach (var _fabricContrast in fabricContrasts)
                {

                    var listValue = _fabricContrast.Split("#").ToList();
                    var fabricContrast = new FabricContrast();
                    fabricContrast.MergeBlockLSStyle = dailyTarget.MergeBlockLSStyle;
                    fabricContrast.Name = listValue[0].Trim();
                    fabricContrast.ContrastColor = listValue[1].Trim() + "/ " + colorCode;
                    if (listValue.Count() > 2)
                    {
                        fabricContrast.Description = listValue[2].Trim();
                        fabricContrast.DescriptionForShirt = fabricContrast.DescriptionForPant = fabricContrast.Description;
                    }
                    //
                    var existFabricContrast = context.FabricContrast.Where(x => x.MergeBlockLSStyle == dailyTarget.MergeBlockLSStyle
                                                                            && x.Name == fabricContrast.Name
                                                                ).ToList();

                    //
                    if (existFabricContrast.Count() == 0)
                    {
                        fabricContrast.SetCreateAudit(userName);
                        context.FabricContrast.Add(fabricContrast);
                    }
                }
                context.SaveChanges();
            }
        }
        public class SampleSize
        {
            public string Size { get; set; }
            public decimal SampleQuantity { get; set; }
        }
    }
}
