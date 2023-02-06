using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Ressult;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
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
    public class CancelItemStyleCommandHandler
        : IRequestHandler<CancelItemStyleCommand, CancelItemStyleResult>
    {
        private readonly ILogger<CancelItemStyleCommandHandler> logger;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly IReservationEntryRepository reservationEntryRepository;
        private readonly IPurchaseOrderLineRepository purchaseOrderLineRepository;
        private readonly SqlServerAppDbContext context;

        public CancelItemStyleCommandHandler(ILogger<CancelItemStyleCommandHandler> logger,
            IItemStyleRepository itemStyleRepository,
            IReservationEntryRepository reservationEntryRepository,
            IPurchaseOrderLineRepository purchaseOrderLineRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.itemStyleRepository = itemStyleRepository;
            this.reservationEntryRepository = reservationEntryRepository;
            this.purchaseOrderLineRepository = purchaseOrderLineRepository;
            this.context = context;
        }
        public async Task<CancelItemStyleResult> Handle(
            CancelItemStyleCommand request, CancellationToken cancellationToken)
        {
            var result = new CancelItemStyleResult();

            logger.LogInformation("{@time} - Execute cancel itemstyle command", DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Execute cancel itemstyle command",
                DateTime.Now.ToString());

            var itemStyles = itemStyleRepository
                .GetItemStyles(request.StyleNumbers);
            
            var productionBOMs = itemStyles
                .SelectMany(x => x.ProductionBOMs);

            var reservationEntries = reservationEntryRepository
                .GetReservationEntries();

            CancelItemStyleProcess.BulkCancel(request.Username, itemStyles.ToList(), 
                productionBOMs.ToList(),
                reservationEntries.ToList(), 
                out List<ReservationEntry> willDeleteReservationEntries,
                out List<PurchaseOrderLine> willUpdatePurchaseOrderLines);

            try
            {
                context.ItemStyle.UpdateRange(itemStyles);
                context.ReservationEntry.RemoveRange(willDeleteReservationEntries);
                context.PurchaseOrderLine.UpdateRange(willUpdatePurchaseOrderLines);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
                logger.LogInformation("{@time} - Execute cancel itemstyle command", DateTime.Now.ToString());
                LogHelper.Instance.Information("{@time} - Execute cancel itemstyle command",
                    DateTime.Now.ToString());
            }

            return result;
        }
    }
}
