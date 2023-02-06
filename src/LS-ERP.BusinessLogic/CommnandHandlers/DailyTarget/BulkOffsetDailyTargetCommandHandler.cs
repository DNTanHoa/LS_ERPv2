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
    public class BulkOffsetDailyTargetCommandHandler : IRequestHandler<BulkOffsetDailyTargetCommand,
        CommonCommandResultHasData<IEnumerable<DailyTarget>>>
    {
        private readonly ILogger<BulkOffsetDailyTargetCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public BulkOffsetDailyTargetCommandHandler(ILogger<BulkOffsetDailyTargetCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<IEnumerable<DailyTarget>>> Handle(
            BulkOffsetDailyTargetCommand request, 
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
                var dailyTargetModels = request.Data;
                var dailyTargets = request.Data
                    .Select(x => mapper.Map<DailyTarget>(x)).ToList();
                var lsStyles = dailyTargetModels.Select(x => x.LSStyle).Distinct().ToList();
                foreach (var lsstyle in lsStyles)
                {
                    var allocDailyOutputs = dailyTargetModels.Where(d => d.LSStyle == lsstyle).Select(x => mapper.Map<AllocDailyOutput>(x)).ToList();
                    var dailyTarget = mapper.Map<DailyTarget>(dailyTargetModels.Where(d => d.LSStyle == lsstyle).FirstOrDefault());
                    var existDailyTarget = context.DailyTarget.Where(x => x.Operation == "CUTTING"
                                                                            && x.LSStyle == dailyTarget.LSStyle
                                                                            && x.CompanyID == dailyTarget.CompanyID
                                                                            ).FirstOrDefault();
                    if (existDailyTarget != null)
                    {
                        //context.DailyTarget.Remove(existDailyTarget);
                    }
                    else
                    {
                        // set targetQuantity
                        dailyTarget.TargetQuantity = dailyTargetModels.Where(d => d.LSStyle == dailyTarget.LSStyle).Select(s => s.TargetQuantity).Sum();
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
                        //
                        dailyTarget.IsCreatedDetail = true;
                        dailyTarget.Remark = "Import offset by "+request.UserName;
                        dailyTarget.SetCreateAudit(request.UserName);
                        context.DailyTarget.Add(dailyTarget);
                        context.SaveChanges();
                        /////
                        ///
                        //Create FabricContrast if not Exist
                        //if (CheckNotExistFabricContrast(dailyTarget))
                        //{
                            CreateListFabricContrast(dailyTarget, request.UserName);
                        //}
                        var Sets = dailyTarget.Set.Split(";").ToList();
                        foreach (var set in Sets)
                        {
                            foreach (var allocDailyOutput in allocDailyOutputs)
                            {
                                var fabricContrasts = context.FabricContrast.Where(x => x.MergeBlockLSStyle == dailyTarget.MergeBlockLSStyle).ToList();
                                foreach (var fabricContrast in fabricContrasts)
                                {
                                    var newAllocDailyOutput = allocDailyOutput;
                                    newAllocDailyOutput.ID = 0;
                                    newAllocDailyOutput.TargetID = dailyTarget.ID;
                                    newAllocDailyOutput.Set = set;
                                    newAllocDailyOutput.FabricContrastID = fabricContrast.ID;
                                    newAllocDailyOutput.FabricContrastName = fabricContrast.Name;
                                    newAllocDailyOutput.SetCreateAudit(request.UserName);
                                    context.AllocDailyOutput.Add(newAllocDailyOutput);
                                    context.SaveChanges();
                                }

                            }
                        }
                    }
                }
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
        //create listON for MergeBlockLSStyle
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
        public bool CheckNotExistFabricContrast(DailyTarget dailyTarget)
        {
            var result = false;
            var fabricContrast = context.FabricContrast.Where(x => x.MergeBlockLSStyle == dailyTarget.MergeBlockLSStyle).ToList();
            if (fabricContrast.Count == 0)
            {
                result = true;
            }
            return result;
        }

    }
}
