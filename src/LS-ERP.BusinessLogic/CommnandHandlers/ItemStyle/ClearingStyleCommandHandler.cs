using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Ressult;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
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
    public class ClearingStyleCommandHandler
        : IRequestHandler<ClearingStyleCommand, ClearingStyleResult>
    {
        private readonly ILogger<ClearingStyleCommandHandler> logger;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly IStorageDetailRepository storageDetailRepository;
        private readonly SqlServerAppDbContext context;

        public ClearingStyleCommandHandler(ILogger<ClearingStyleCommandHandler> logger,
            IItemStyleRepository itemStyleRepository,
            IStorageDetailRepository storageDetailRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.itemStyleRepository = itemStyleRepository;
            this.storageDetailRepository = storageDetailRepository;
            this.context = context;
        }

        public async Task<ClearingStyleResult> Handle(ClearingStyleCommand request, CancellationToken cancellationToken)
        {
            var result = new ClearingStyleResult();

            logger.LogInformation("{@time} - Exceute clearing style for item style command",
               DateTime.Now.ToString());

            var itemStyles = itemStyleRepository.GetItemStyles(request.ItemStyleNumbers.ToList());
            var finishGoods = storageDetailRepository.GetStorageDetails("FG");

            ClearingProcess.ClearingStyle(itemStyles.ToList(), finishGoods.ToList(), 
                out List<ReservationEntry> reservationEntries);

            try
            {
                context.ReservationEntry.AddRange(reservationEntries);
                context.StorageDetail.UpdateRange(finishGoods);

                await context.SaveChangesAsync(cancellationToken);

                result.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException?.Message;
            }

            return result;
        }
    }
}
