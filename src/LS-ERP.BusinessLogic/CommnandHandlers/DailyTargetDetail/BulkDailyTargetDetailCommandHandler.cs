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
    public class BulkDailyTargetDetailCommandHandler : IRequestHandler<BulkDailyTargetDetailCommand,
        CommonCommandResultHasData<IEnumerable<DailyTargetDetail>>>
    {
        private readonly ILogger<BulkDailyTargetDetailCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public BulkDailyTargetDetailCommandHandler(ILogger<BulkDailyTargetDetailCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<IEnumerable<DailyTargetDetail>>> Handle(
            BulkDailyTargetDetailCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<IEnumerable<DailyTargetDetail>>();
            logger.LogInformation("{@time} - Exceute bulk job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute bulk job output command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var dailyTargetDetails = request.Data
                    .Select(x => mapper.Map<DailyTargetDetail>(x)).ToList();
                
                foreach(var dailyTargetDetail in dailyTargetDetails)
                {
                    // Set Operation
                    dailyTargetDetail.Operation = "CUTTING";
                    dailyTargetDetail.IsAllocated = true;
                    //remove DailyTargetDetail if exist
                    var produceDate = dailyTargetDetail.ProduceDate;                    
                    var existDailyTargetDetail = context.DailyTargetDetail.Where(x => x.WorkCenterID == dailyTargetDetail.WorkCenterID
                                                                             && x.ProduceDate.Day == produceDate.Day
                                                                             && x.ProduceDate.Month == produceDate.Month
                                                                             && x.ProduceDate.Year == produceDate.Year 
                                                                             && x.Operation == "CUTTING"
                                                                             && x.Size.Equals(dailyTargetDetail.Size)
                                                                             && x.LSStyle == dailyTargetDetail.LSStyle
                                                                        ).FirstOrDefault();
                    if(existDailyTargetDetail != null)
                    {
                        context.DailyTargetDetail.Remove(existDailyTargetDetail);
                    }
                    //
                    dailyTargetDetail.SetCreateAudit(request.UserName);
                    context.DailyTargetDetail.Add(dailyTargetDetail);
                    context.SaveChanges();
                }
                result.Success = true;
                result.Data = dailyTargetDetails;
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute bulk job output command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute bulk job output command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
        
    }
}
