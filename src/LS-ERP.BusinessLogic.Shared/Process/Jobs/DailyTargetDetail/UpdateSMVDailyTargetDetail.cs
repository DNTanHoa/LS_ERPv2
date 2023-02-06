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
    public class UpdateSMVDailyTargetDetail
    {
        private readonly ILogger<UpdateSMVDailyTargetDetail> logger;
        private readonly SqlServerAppDbContext context;

        public UpdateSMVDailyTargetDetail(ILogger<UpdateSMVDailyTargetDetail> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [JobDisplayName("Update smv daily target detail")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute(PartPrice partPrice)
        {
            try
            {
                var dailyTargetDetails = context.DailyTargetDetail.Where(x => x.WorkCenterID.Contains(partPrice.CompanyID)
                                                                      && x.StyleNO == partPrice.StyleNO
                                                                      && x.Item == partPrice.Item
                                                                      && x.ProduceDate >= partPrice.EffectiveDate).ToList();
                foreach (var dailyTargetDetail in dailyTargetDetails)
                {
                    dailyTargetDetail.SMV = partPrice.SMV;
                    var totalTargetQuantity = Math.Round(11 * 60 * dailyTargetDetail.NumberOfWorker / partPrice.SMV);
                    dailyTargetDetail.TotalTargetQuantity = totalTargetQuantity;

                    //Calc Efficiency Daily Target Detail
                    if (dailyTargetDetail.Quantity > 0)
                    {
                        dailyTargetDetail.Efficiency = dailyTargetDetail.Quantity / totalTargetQuantity;
                    }

                    context.Update(dailyTargetDetail);
                }
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("Daily target detail - update smv event handler has error with message {@message}",
                    ex.InnerException?.Message);
                LogHelper.Instance.Error("Daily target detail - update smv event handler has error with message {@message}",
                    ex.InnerException?.Message);
            }

            return Task.CompletedTask;
        }
    }
}
