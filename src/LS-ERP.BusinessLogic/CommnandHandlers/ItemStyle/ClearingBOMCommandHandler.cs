using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Ressult;
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
    public class ClearingBOMCommandHandler : IRequestHandler<ClearingBOMCommand, ClearingBOMResult>
    {
        private readonly ILogger<ClearingBOMCommandHandler> logger;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly IProductionBOMRepository productionBOMRepository;
        private readonly IStorageDetailRepository storageDetailRepository;
        private readonly SqlServerAppDbContext context;

        public ClearingBOMCommandHandler(ILogger<ClearingBOMCommandHandler> logger,
            IItemStyleRepository itemStyleRepository,
            IProductionBOMRepository productionBOMRepository,
            IStorageDetailRepository storageDetailRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.itemStyleRepository = itemStyleRepository;
            this.productionBOMRepository = productionBOMRepository;
            this.storageDetailRepository = storageDetailRepository;
            this.context = context;
        }

        public async Task<ClearingBOMResult> Handle(ClearingBOMCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new ClearingBOMResult();

            var productionBOMs = productionBOMRepository
                .GetProductionBOMsOfItemStyles(request.ItemStyleNumbers.ToList());

            var storageDetails = storageDetailRepository
                .GetStorageDetails()
                .Where(x => x.CanReUseQuantity > 0);

            try
            {

                await context.SaveChangesAsync(cancellationToken);
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.InnerException?.Message;
            }

            return result;
        }
    }
}
