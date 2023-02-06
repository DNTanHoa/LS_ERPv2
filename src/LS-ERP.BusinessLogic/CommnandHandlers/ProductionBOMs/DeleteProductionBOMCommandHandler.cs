using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
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
    public class DeleteProductionBOMCommandHandler
        : IRequestHandler<DeleteProductionBOMCommand, DeleteProductionBOMResult>
    {
        private readonly ILogger<DeleteProductionBOMCommandHandler> logger;
        private readonly IProductionBOMRepository productionBOMRepository;
        private readonly SqlServerAppDbContext context;

        public DeleteProductionBOMCommandHandler(ILogger<DeleteProductionBOMCommandHandler> logger,
            IProductionBOMRepository productionBOMRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.productionBOMRepository = productionBOMRepository;
            this.context = context;
        }

        public async Task<DeleteProductionBOMResult> Handle(DeleteProductionBOMCommand request, CancellationToken cancellationToken)
        {
            var result = new DeleteProductionBOMResult();

            var willDeleteProductionBOMs = productionBOMRepository
                .GetProductionBOMs(request.ProductionBOMs.Select(x => x.ID).Distinct().ToList());

            var purchasedProductionBOM = willDeleteProductionBOMs.FirstOrDefault(x => x.ReservedQuantity > 0);

            if(purchasedProductionBOM != null)
            {
                result.Message = "Production bom was purchase can't be delete";
                return result;
            }

            context.ProductionBOM.RemoveRange(willDeleteProductionBOMs);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
