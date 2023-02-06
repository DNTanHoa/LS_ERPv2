using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Ressult;
using LS_ERP.BusinessLogic.Shared.Process;
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
    public class CalculateRequiredQuantityCommandHandler
        : IRequestHandler<CalculateRequiredQuantityCommand, CalculateRequiredQuantityResult>
    {
        private readonly ILogger<CalculateRequiredQuantityCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly IProductionBOMRepository productionBOMRepository;
        private readonly IWastageSettingRepository wastageSettingRepository;

        public CalculateRequiredQuantityCommandHandler(ILogger<CalculateRequiredQuantityCommandHandler> logger,
            SqlServerAppDbContext context,
            IItemStyleRepository itemStyleRepository,
            IProductionBOMRepository productionBOMRepository,
            IWastageSettingRepository wastageSettingRepository)
        {
            this.logger = logger;
            this.context = context;
            this.itemStyleRepository = itemStyleRepository;
            this.productionBOMRepository = productionBOMRepository;
            this.wastageSettingRepository = wastageSettingRepository;
        }

        public async Task<CalculateRequiredQuantityResult> Handle(CalculateRequiredQuantityCommand request, CancellationToken cancellationToken)
        {
            var result = new CalculateRequiredQuantityResult();

            logger.LogInformation("{@time} - Exceute calculate required quantity for item style command", 
                DateTime.Now.ToString());

            var productionBOMs = productionBOMRepository.GetProductionBOMsOfItemStyles(request.StyleNumbers);
            bool calculateResult = false;
            string errorMessage = string.Empty;

            switch (request.CustomerID)
            {
                case "HA":
                    var wastageSettings = wastageSettingRepository.GetWastageSettings();
                    calculateResult =
                        PullBomItemStyleProcess.CalculateRequiredQuantity(productionBOMs.ToList(), true, request.Username, wastageSettings.ToList(),
                                out errorMessage);
                    break;
                default:
                    calculateResult =
                        PullBomItemStyleProcess.CalculateRequiredQuantity(productionBOMs.ToList(), false, request.Username, null, out errorMessage);
                    break;
            }

            if (!calculateResult)
            {
                result.Message = errorMessage;
                return result;
            }

            try
            {
                context.ProductionBOM.UpdateRange(productionBOMs);
                await context.SaveChangesAsync();

                result.IsSuccess = true;
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.InnerException?.Message;
            }

            return result;
        }
    }
}
