using AutoMapper;
using Common.Model;
using LS_ERP.BusinessLogic.Commands;
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
    public class BulkOffsetDailyTargetDetailCommandHandler : IRequestHandler<BulkOffsetDailyTargetDetailCommand,
        CommonCommandResultHasData<IEnumerable<DailyTargetDetail>>>
    {
        private readonly ILogger<BulkOffsetDailyTargetDetailCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public BulkOffsetDailyTargetDetailCommandHandler(ILogger<BulkOffsetDailyTargetDetailCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<IEnumerable<DailyTargetDetail>>> Handle(
            BulkOffsetDailyTargetDetailCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<IEnumerable<DailyTargetDetail>>();
            logger.LogInformation("{@time} - Exceute bulk offset daily target detail command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute bulk offset daily target detail command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var dailyTargetDetails = request.Data
                    .Select(x => mapper.Map<DailyTargetDetail>(x)).ToList();

                foreach (var dailyTargetDetail in dailyTargetDetails)
                {
                    var produceDate = dailyTargetDetail.ProduceDate;
                    var existDailyTargetDetail = context.DailyTargetDetail.Where(x => x.WorkCenterID.Contains(dailyTargetDetail.WorkCenterID)
                                                                             && x.WorkCenterName == dailyTargetDetail.WorkCenterName
                                                                             && x.StyleNO == dailyTargetDetail.StyleNO
                                                                             && x.Item == dailyTargetDetail.Item
                                                                             && x.ProduceDate.Day == produceDate.Day
                                                                             && x.ProduceDate.Month == produceDate.Month
                                                                             && x.ProduceDate.Year == produceDate.Year
                                                                        ).FirstOrDefault();

                    if (existDailyTargetDetail != null)
                    {
                        if (dailyTargetDetail.Offset>0)
                        {
                            existDailyTargetDetail.Offset = dailyTargetDetail.Offset;
                        }
                        if (dailyTargetDetail.RejectRate>0)
                        {
                            existDailyTargetDetail.RejectRate = dailyTargetDetail.RejectRate;
                        }
                        
                        existDailyTargetDetail.SetUpdateAudit(request.UserName);
                        context.DailyTargetDetail.Update(existDailyTargetDetail);
                    }
                }
                context.SaveChanges();
                result.Success = true;
                result.Data = dailyTargetDetails;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute bulk offset daily target detail command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute bulk offset daily target detail command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }

    }
}
