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
    public class CreatePartPriceJob
    {
        private readonly ILogger<CreatePartPriceJob> logger;
        private readonly SqlServerAppDbContext context;

        public CreatePartPriceJob(ILogger<CreatePartPriceJob> logger, SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }
        [JobDisplayName("Create part price when insert/update DailyTargetDetail")]
        [AutomaticRetry(Attempts =3)]
        public Task Execute(DailyTargetDetail dailyTargetDetail)
        {
            try
            {
                var existPartPrice = context.PartPrice.Where(x => x.CompanyID.Contains(dailyTargetDetail.WorkCenterID.Substring(0,2))
                                                                      && x.StyleNO == dailyTargetDetail.StyleNO
                                                                      && x.Item == dailyTargetDetail.Item).FirstOrDefault();

                if (existPartPrice == null)
                {
                    var partPrice = new PartPrice()
                    {
                        CompanyID = dailyTargetDetail.WorkCenterID.Substring(0, 2),
                        StyleNO = dailyTargetDetail.StyleNO,
                        Item = dailyTargetDetail.Item,
                        SMV = (decimal)dailyTargetDetail.SMV,
                        EffectiveDate = dailyTargetDetail.ProduceDate,
                        ExpiryDate = DateTime.MaxValue,
                        Remark = dailyTargetDetail.Remark,
                    };
                    partPrice.SetCreateAudit(dailyTargetDetail.CreatedBy);
                    context.Add(partPrice);
                }
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("Create part price when insert/update DailyTargetDetail event handler has error with message {@message}",
                    ex.InnerException?.Message);
                LogHelper.Instance.Error("Create part price when insert/update DailyTargetDetail event handler has error with message {@message}",
                    ex.InnerException?.Message);
            }

            return Task.CompletedTask;
        }

        [JobDisplayName("Create part price when insert/update DailyTarget")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute(DailyTarget dailyTarget)
        {
            try
            {
                var existPartPrice = context.PartPrice.Where(x => x.CompanyID.Contains(dailyTarget.WorkCenterID.Substring(0, 2))
                                                                      && x.StyleNO == dailyTarget.StyleNO
                                                                      && x.Item == dailyTarget.Item).FirstOrDefault();

                if (existPartPrice == null)
                {
                    var partPrice = new PartPrice()
                    {
                        CompanyID = dailyTarget.WorkCenterID.Substring(0, 2),
                        StyleNO = dailyTarget.StyleNO,
                        Item = dailyTarget.Item,
                        SMV = (decimal)dailyTarget.SMV,
                        EffectiveDate = dailyTarget.ProduceDate,
                        ExpiryDate = DateTime.MaxValue,
                        Remark = dailyTarget.Remark,
                    };
                    partPrice.SetCreateAudit(dailyTarget.CreatedBy);
                    context.Add(partPrice);
                }
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("Create part price when insert/update DailyTarget event handler has error with message {@message}",
                    ex.InnerException?.Message);
                LogHelper.Instance.Error("Create part price when insert/update DailyTarget event handler has error with message {@message}",
                    ex.InnerException?.Message);
            }

            return Task.CompletedTask;
        }
    }
}
