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
    public class CreateCuttingOutputCommandHandler
        : IRequestHandler<CreateCuttingOutputCommand, CommonCommandResultHasData<CuttingOutput>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateCuttingOutputCommandHandler> logger;
        private readonly IMapper mapper;

        public CreateCuttingOutputCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateCuttingOutputCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<CuttingOutput>> Handle(CreateCuttingOutputCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<CuttingOutput>();
            logger.LogInformation("{@time} - Exceute create cutting output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute create cutting output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());            
            try
            {
                var cuttingOutputs = new List<CuttingOutput>();
                if(request.CuttingLot.Count() > 0)
                {
                    foreach(var cuttingLot in request.CuttingLot)
                    {
                        var listSizeRatio = request.ListSizeRatio.Where(x => x.Ratio != null);
                        if (listSizeRatio.Count() > 0)
                        {
                            foreach (var sizeRatio in listSizeRatio)
                            {
                                //
                                var cuttingOutput = mapper.Map<CuttingOutput>(request);
                                //
                                cuttingOutput.Lot = cuttingLot.CuttingLot;
                                cuttingOutput.Layer = cuttingLot.CuttingLayer;
                                //
                                cuttingOutput.Size = sizeRatio.Size;
                                cuttingOutput.Ratio = (decimal)sizeRatio.Ratio;
                                cuttingOutput.Quantity = (decimal)sizeRatio.Ratio*cuttingLot.CuttingLayer;
                                //
                                var workCenter = context.WorkCenter.Where(x => x.ID == cuttingOutput.WorkCenterID).FirstOrDefault();
                                cuttingOutput.WorkCenterName = workCenter.Name;
                                var fabricContrast = context.FabricContrast.Where(x => x.ID == cuttingOutput.FabricContrastID).FirstOrDefault();
                                cuttingOutput.FabricContrast = fabricContrast.Name;
                                cuttingOutput.FabricContrastDescription = fabricContrast.Description;
                                //set
                                var dailyTarget = context.DailyTarget.Where(x => x.MergeBlockLSStyle == cuttingOutput.MergeBlockLSStyle
                                                                            && x.Operation == "CUTTING").FirstOrDefault();
                                cuttingOutput.CustomerID = dailyTarget.CustomerID;
                                cuttingOutput.CustomerName = dailyTarget.CustomerName;
                                //
                                cuttingOutput.SetCreateAudit(request.UserName);
                                context.CuttingOutput.Add(cuttingOutput);
                                context.SaveChanges();
                                result.Success = true;
                                result.SetData(cuttingOutput);
                                //Call job alloc Cutting Output
                                cuttingOutputs.Add(cuttingOutput);
                                //var jobId = BackgroundJob.Enqueue<AllocateCuttingOutputJob>(j => j.Execute(cuttingOutput, request.UserName));
                            }
                        }
                        else
                        {
                            //
                            var cuttingOutput = mapper.Map<CuttingOutput>(request);
                            var workCenter = context.WorkCenter.Where(x => x.ID == cuttingOutput.WorkCenterID).FirstOrDefault();
                            cuttingOutput.WorkCenterName = workCenter.Name;
                            var fabricContrast = context.FabricContrast.Where(x => x.ID == cuttingOutput.FabricContrastID).FirstOrDefault();
                            cuttingOutput.FabricContrast = fabricContrast.Name;
                            cuttingOutput.FabricContrastDescription = fabricContrast.Description;
                            //set
                            var dailyTarget = context.DailyTarget.Where(x => x.MergeBlockLSStyle == cuttingOutput.MergeBlockLSStyle
                                                                        && x.Operation == "CUTTING").FirstOrDefault();
                            cuttingOutput.CustomerID = dailyTarget.CustomerID;
                            cuttingOutput.CustomerName = dailyTarget.CustomerName;
                            //
                            cuttingOutput.Lot = cuttingLot.CuttingLot;
                            cuttingOutput.Layer = cuttingLot.CuttingLayer;
                            cuttingOutput.Quantity = (decimal)cuttingOutput.Ratio * cuttingLot.CuttingLayer;
                            //
                            cuttingOutput.SetCreateAudit(request.UserName);
                            context.CuttingOutput.Add(cuttingOutput);
                            context.SaveChanges();
                            result.Success = true;
                            result.SetData(cuttingOutput);
                            //Call job alloc Cutting Output
                            cuttingOutputs.Add(cuttingOutput);
                            //var jobId = BackgroundJob.Enqueue<AllocateCuttingOutputJob>(j => j.Execute(cuttingOutput, request.UserName));
                        }
                    }    
                }   
                else
                {
                    var listSizeRatio = request.ListSizeRatio.Where(x => x.Ratio != null);
                    if (listSizeRatio.Count() > 0)
                    {
                        foreach (var sizeRatio in listSizeRatio)
                        {
                            //
                            var cuttingOutput = mapper.Map<CuttingOutput>(request);
                            //
                            cuttingOutput.Size = sizeRatio.Size;
                            cuttingOutput.Ratio = (decimal)sizeRatio.Ratio;
                            cuttingOutput.Quantity = (decimal)sizeRatio.Ratio* cuttingOutput.Layer;
                            //
                            var workCenter = context.WorkCenter.Where(x => x.ID == cuttingOutput.WorkCenterID).FirstOrDefault();
                            cuttingOutput.WorkCenterName = workCenter.Name;
                            var fabricContrast = context.FabricContrast.Where(x => x.ID == cuttingOutput.FabricContrastID).FirstOrDefault();
                            cuttingOutput.FabricContrast = fabricContrast.Name;
                            cuttingOutput.FabricContrastDescription = fabricContrast.Description;
                            //set
                            var dailyTarget = context.DailyTarget.Where(x => x.MergeBlockLSStyle == cuttingOutput.MergeBlockLSStyle
                                                                        && x.Operation == "CUTTING").FirstOrDefault();
                            cuttingOutput.CustomerID = dailyTarget.CustomerID;
                            cuttingOutput.CustomerName = dailyTarget.CustomerName;
                            //
                            cuttingOutput.SetCreateAudit(request.UserName);
                            context.CuttingOutput.Add(cuttingOutput);
                            context.SaveChanges();
                            result.Success = true;
                            result.SetData(cuttingOutput);
                            //Call job alloc Cutting Output
                            //var jobId = BackgroundJob.Enqueue<AllocateCuttingOutputJob>(j => j.Execute(cuttingOutput, request.UserName));
                            cuttingOutputs.Add(cuttingOutput);
                        }
                    }
                    else
                    {
                        //
                        var cuttingOutput = mapper.Map<CuttingOutput>(request);
                        var workCenter = context.WorkCenter.Where(x => x.ID == cuttingOutput.WorkCenterID).FirstOrDefault();
                        cuttingOutput.WorkCenterName = workCenter.Name;
                        var fabricContrast = context.FabricContrast.Where(x => x.ID == cuttingOutput.FabricContrastID).FirstOrDefault();
                        cuttingOutput.FabricContrast = fabricContrast.Name;
                        cuttingOutput.FabricContrastDescription = fabricContrast.Description;
                        //set
                        var dailyTarget = context.DailyTarget.Where(x => x.MergeBlockLSStyle == cuttingOutput.MergeBlockLSStyle
                                                                    && x.Operation == "CUTTING").FirstOrDefault();
                        cuttingOutput.CustomerID = dailyTarget.CustomerID;
                        cuttingOutput.CustomerName = dailyTarget.CustomerName;
                        //
                        cuttingOutput.SetCreateAudit(request.UserName);
                        context.CuttingOutput.Add(cuttingOutput);
                        context.SaveChanges();
                        result.Success = true;
                        result.SetData(cuttingOutput);
                        //Call job alloc Cutting Output
                        //var jobId = BackgroundJob.Enqueue<AllocateCuttingOutputJob>(j => j.Execute(cuttingOutput, request.UserName));
                        cuttingOutputs.Add(cuttingOutput);
                    }
                }
                var jobId = BackgroundJob.Enqueue<AllocateCuttingOutputJob>(j => j.Execute(cuttingOutputs, request.UserName));
                //var listSizeRatio = request.ListSizeRatio.Where(x => x.Ratio != null);
                //if (listSizeRatio.Count() > 0)
                //{
                //    foreach (var sizeRatio in listSizeRatio)
                //    {
                //        //
                //        var cuttingOutput = mapper.Map<CuttingOutput>(request);
                //        //
                //        cuttingOutput.Size = sizeRatio.Size;
                //        cuttingOutput.Ratio = (decimal)sizeRatio.Ratio;
                //        cuttingOutput.Quantity = (decimal)sizeRatio.Quantity;
                //        //
                //        var workCenter = context.WorkCenter.Where(x => x.ID == cuttingOutput.WorkCenterID).FirstOrDefault();
                //        cuttingOutput.WorkCenterName = workCenter.Name;
                //        var fabricContrast = context.FabricContrast.Where(x => x.ID == cuttingOutput.FabricContrastID).FirstOrDefault();
                //        cuttingOutput.FabricContrast = fabricContrast.Name;
                //        cuttingOutput.FabricContrastDescription = fabricContrast.Description;
                //        //set
                //        var dailyTarget = context.DailyTarget.Where(x => x.MergeBlockLSStyle == cuttingOutput.MergeBlockLSStyle
                //                                                    && x.Operation == "CUTTING").FirstOrDefault();
                //        cuttingOutput.CustomerID = dailyTarget.CustomerID;
                //        cuttingOutput.CustomerName = dailyTarget.CustomerName;
                //        //
                //        cuttingOutput.SetCreateAudit(request.UserName);
                //        context.CuttingOutput.Add(cuttingOutput);
                //        context.SaveChanges();
                //        result.Success = true;
                //        result.SetData(cuttingOutput);
                //        //Call job alloc Cutting Output
                //        var jobId = BackgroundJob.Enqueue<AllocateCuttingOutputJob>(j => j.Execute(cuttingOutput, request.UserName));
                //    }
                //}
                //else
                //{
                //    //
                //    var cuttingOutput = mapper.Map<CuttingOutput>(request);
                //    var workCenter = context.WorkCenter.Where(x => x.ID == cuttingOutput.WorkCenterID).FirstOrDefault();
                //    cuttingOutput.WorkCenterName = workCenter.Name;
                //    var fabricContrast = context.FabricContrast.Where(x => x.ID == cuttingOutput.FabricContrastID).FirstOrDefault();
                //    cuttingOutput.FabricContrast = fabricContrast.Name;
                //    cuttingOutput.FabricContrastDescription = fabricContrast.Description;
                //    //set
                //    var dailyTarget = context.DailyTarget.Where(x => x.MergeBlockLSStyle == cuttingOutput.MergeBlockLSStyle
                //                                                && x.Operation == "CUTTING").FirstOrDefault();
                //    cuttingOutput.CustomerID = dailyTarget.CustomerID;
                //    cuttingOutput.CustomerName = dailyTarget.CustomerName;
                //    //
                //    cuttingOutput.SetCreateAudit(request.UserName);
                //    context.CuttingOutput.Add(cuttingOutput);
                //    context.SaveChanges();
                //    result.Success = true;
                //    result.SetData(cuttingOutput);
                //    //Call job alloc Cutting Output
                //    var jobId = BackgroundJob.Enqueue<AllocateCuttingOutputJob>(j => j.Execute(cuttingOutput, request.UserName));
                //}

            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute create cutting output command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute create cutting output command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }
            return Task.FromResult(result);
        }
       
    }
}
