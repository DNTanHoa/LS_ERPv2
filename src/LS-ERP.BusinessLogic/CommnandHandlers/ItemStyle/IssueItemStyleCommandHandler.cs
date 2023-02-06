using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class IssueItemStyleCommandHandler : IRequestHandler<IssueItemStyleCommand, CommonCommandResult>
    {
        private readonly ILogger<IssueItemStyleCommandHandler> logger;
        private readonly SqlServerAppDbContext context;

        public IssueItemStyleCommandHandler(ILogger<IssueItemStyleCommandHandler> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }
        public async Task<CommonCommandResult> Handle(IssueItemStyleCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            /// Lấy dữ liệu item style
            var itemStyles = context.ItemStyle
                .Where(x => request.ItemStyleNumbers.Contains(x.Number))
                .ToList();
            var itemStyleSyncMasters = context.ItemStyleSyncMasters
                .Include(x => x.ItemStyle)
                .Where(x => itemStyles.Where(x => x.IsIssued != true)
                    .Select(x => x.Number).Contains(x.ItemStyleNumber))
                .ToList();

            try
            {
                itemStyles.ForEach(x =>
                {
                    x.IsIssued = true;
                });
                itemStyleSyncMasters.ForEach(x =>
                {
                    x.IssuedDate = DateTime.Now;
                    x.ProductionSkedDeliveryDate = x.ItemStyle.ShipDate?.AddDays(-10);
                    x.ContractualSupplierHandover = x.ItemStyle.ShipDate.Value;
                    x.Monthly = DateTime.Now.ToString("MMMM");
                });

                context.ItemStyle.UpdateRange(itemStyles);
                context.ItemStyleSyncMasters.UpdateRange(itemStyleSyncMasters);
                
                context.SaveChanges();
                result.Success = true;
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogError("{@time} - " +
                    "Issue item style with request {@request} has error with message {@message}",
                    DateTime.Now.ToString(), JsonConvert.SerializeObject(request), ex.InnerException?.Message);
                LogHelper.Instance.Error("{@time} - " +
                    "Issue item style with request {@request} has error with message {@message}",
                    DateTime.Now.ToString(), JsonConvert.SerializeObject(request), ex.InnerException?.Message);
            }

            return result;
        }
    }
}
