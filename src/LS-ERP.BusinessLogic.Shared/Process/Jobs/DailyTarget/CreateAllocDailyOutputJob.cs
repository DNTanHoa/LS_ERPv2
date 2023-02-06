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
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class CreateAllocDailyOutputJob
    {
        private readonly ILogger<CreateAllocDailyOutputJob> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public CreateAllocDailyOutputJob(ILogger<CreateAllocDailyOutputJob> logger,
            SqlServerAppDbContext context, IMapper mapper) 
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        [JobDisplayName("Create AllocDailyOutput when import Target Cutting")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute(string userName)
        {
            /// Update handle
            try
            {
                var dailyTargets = context.DailyTarget.Where(x => x.IsCreatedDetail == false
                                                                        && x.Operation =="CUTTING"
                                                                        && x.IsDeleted == false
                                                                        ).ToList();
                foreach(var dailyTarget in dailyTargets)
                {
                    //Create FabricContrast if not Exist
                    //if(CheckNotExistFabricContrast(dailyTarget))
                    //{
                    CreateListFabricContrast(dailyTarget,userName);
                    //}
                    var Sets = dailyTarget.Set.Split(";").ToList();
                    var sampleSizes = new List<SampleSize>();
                    if(!string.IsNullOrEmpty(dailyTarget.SampleWithSize))
                    {
                        var listSample = dailyTarget.SampleWithSize.Split(";").ToList();
                        foreach (var sample in listSample)
                        {
                            var s = sample.Split("#").ToList();
                            if(s.Count>=2)
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
                    bool isDetailCreated = false;
                    //
                    var existAllocDailyOutputs = context.AllocDailyOutput.Where(x => x.TargetID == dailyTarget.ID).ToList();
                    if (existAllocDailyOutputs.Count()==0)
                    {
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
                                    var allocDailyOutput = mapper.Map<AllocDailyOutput>(dailyTarget);
                                    allocDailyOutput.TargetID = dailyTarget.ID;
                                    allocDailyOutput.Size = orderDetail.Size.Trim().ToUpper();
                                    allocDailyOutput.Set = set;                                    
                                   // allocDailyOutput.OrderQuantity = (decimal)(orderDetail.Quantity - orderDetail.ConsumedQuantity);
                                   // allocDailyOutput.PercentQuantity =decimal.Round((decimal)(orderDetail.Quantity - orderDetail.ConsumedQuantity) * allocDailyOutput.Percent / 100,0);
                                    if (orderDetail.ConsumedQuantity != null)
                                    {
                                        allocDailyOutput.OrderQuantity = (decimal)(orderDetail.Quantity - orderDetail.ConsumedQuantity!);
                                        allocDailyOutput.PercentQuantity = decimal.Round((decimal)(orderDetail.Quantity - orderDetail.ConsumedQuantity!) * allocDailyOutput.Percent / 100, 0,MidpointRounding.ToPositiveInfinity);
                                    }
                                    else
                                    {
                                        allocDailyOutput.OrderQuantity = (decimal)(orderDetail.Quantity);
                                        allocDailyOutput.PercentQuantity = decimal.Round((decimal)(orderDetail.Quantity) * allocDailyOutput.Percent / 100, 0,MidpointRounding.ToPositiveInfinity);
                                    }
                                    if (dailyTarget.CustomerID != "HA")
                                    {
                                        allocDailyOutput.Sample = sampleSize == null ? dailyTarget.Sample : sampleSize.SampleQuantity;
                                    }    
                                    else
                                    {
                                        allocDailyOutput.Sample = orderDetail.SampleQuantity;
                                    }    
                                    allocDailyOutput.FabricContrastID = fabricContrast.ID;
                                    allocDailyOutput.FabricContrastName = fabricContrast.Name;
                                    if(allocDailyOutput.OrderQuantity == 0)
                                    {
                                        allocDailyOutput.IsFull = true;
                                    }    
                                    allocDailyOutput.SetCreateAudit(userName);
                                    context.AllocDailyOutput.Add(allocDailyOutput);
                                    //
                                    isDetailCreated = true;
                                }
                            }                            
                        }
                    }
                    dailyTarget.IsCreatedDetail = isDetailCreated;
                }
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("Create AllocDailyOutput cutting event handler has error with message {@message}",
                    ex.InnerException?.Message);
                LogHelper.Instance.Error("Create AllocDailyOutput cutting event handler has error with message {@message}",
                    ex.InnerException?.Message);
            }

            return Task.CompletedTask;  
        }

        //create listON for MergeBlockLSStyle
        public void CreateListFabricContrast(DailyTarget dailyTarget,string userName)
        {
            var colorCode = string.Empty;
            colorCode = context.ItemStyle.Where(x=>x.LSStyle == dailyTarget.LSStyle).Select(x=>x.ColorCode).FirstOrDefault();
            if(!string.IsNullOrEmpty(dailyTarget.FabricContrast))
            {
                

                var fabricContrasts = dailyTarget.FabricContrast.Split(";");
                foreach(var _fabricContrast in fabricContrasts)
                {
                   
                    var listValue = _fabricContrast.Split("#").ToList();
                    var fabricContrast = new FabricContrast();
                    fabricContrast.MergeBlockLSStyle = dailyTarget.MergeBlockLSStyle;
                    fabricContrast.Name = listValue[0].Trim();
                    fabricContrast.ContrastColor = listValue[1].Trim() +"/ "+ colorCode;
                    if(listValue.Count() > 2)
                    {
                        fabricContrast.Description = listValue[2].Trim();
                        fabricContrast.DescriptionForShirt = fabricContrast.DescriptionForPant = fabricContrast.Description;
                    }
                    //
                    var existFabricContrast = context.FabricContrast.Where(x => x.MergeBlockLSStyle == dailyTarget.MergeBlockLSStyle
                                                                            && x.Name == fabricContrast.Name
                                                                ).ToList();

                    //
                    if(existFabricContrast.Count() == 0)
                    {
                        fabricContrast.SetCreateAudit(userName);
                        context.FabricContrast.Add(fabricContrast);
                    }  
                }
                context.SaveChanges();
            }
        }
        public bool CheckNotExistFabricContrast(DailyTarget dailyTarget)
        {
            var result = false;
            var fabricContrast = context.FabricContrast.Where(x=>x.MergeBlockLSStyle== dailyTarget.MergeBlockLSStyle 
                                                            ).ToList();
            if(fabricContrast.Count==0)
            {
                result = true;
            }
            return result;
        }
        public class SampleSize
        {
            public string Size { get; set; }
            public decimal SampleQuantity { get; set; }
        }
    }
}
