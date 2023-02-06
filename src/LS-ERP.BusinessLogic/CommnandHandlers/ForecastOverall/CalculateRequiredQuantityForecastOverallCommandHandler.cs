using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
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
    public class CalculateRequiredQuantityForecastOverallCommandHandler
        : IRequestHandler<CalculateRequiredQuantityForecastOverallCommand, CalculateRequiredQuantityForecastOverallResult>
    {
        private readonly ILogger<CalculateRequiredQuantityForecastOverallCommandHandler> logger;
        private readonly IPartRevisionRepository partRevisionRepository;
        private readonly IForecastOverallRepository forecastOverallRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IWastageSettingRepository wastageSettingRepository;
        private readonly SqlServerAppDbContext context;

        public CalculateRequiredQuantityForecastOverallCommandHandler(ILogger<CalculateRequiredQuantityForecastOverallCommandHandler> logger,
            IPartRevisionRepository partRevisionRepository,
            IForecastOverallRepository forecastOverallRepository,
            ICustomerRepository customerRepository,
            IWastageSettingRepository wastageSettingRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.partRevisionRepository = partRevisionRepository;
            this.forecastOverallRepository = forecastOverallRepository;
            this.customerRepository = customerRepository;
            this.wastageSettingRepository = wastageSettingRepository;
            this.context = context;
        }
        public async Task<CalculateRequiredQuantityForecastOverallResult> Handle(CalculateRequiredQuantityForecastOverallCommand request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute calculate required quantity for forecast command", DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Exceute calculate required quantity for forecast command",
                DateTime.Now.ToString());

            var result = new CalculateRequiredQuantityForecastOverallResult();

            if (!customerRepository.IsExist(request.CustomerID))
            {
                result.Message = "Invalid customer";
                return result;
            }

            IQueryable<ForecastOverall> forecastOveralls = null;
            IQueryable<PartRevision> partRevisions = null;

            List<ForecastMaterial> updateForecastMaterials = null;
            List<WastageSetting> wastageSettings = null;
            string errorMessage = string.Empty;

            bool pullBomResult = false;

            switch (request.CustomerID)
            {
                case "GA":
                case "DE":
                    forecastOveralls = forecastOverallRepository.GetForecastOveralls(request.ForecastOverallIDs.ToList());
                    partRevisions = partRevisionRepository.GetLastestPartRevisions(forecastOveralls.Select(x => x.CustomerStyle)
                        .ToList());
                    wastageSettings = wastageSettingRepository.GetWastageSettings(request.CustomerID)
                        .ToList();

                    pullBomResult = PullBOMForecastOverallProcess.CalculateRequiredQuantity(forecastOveralls.ToList(),
                        request.Username, partRevisions.ToList(),
                        wastageSettings, out updateForecastMaterials,
                        out errorMessage);
                    break;
            }

            result.IsSuccess = pullBomResult;

            if (pullBomResult)
            {

                foreach (var overall in forecastOveralls)
                {
                    overall.IsBomPulled = true;
                    overall.IsQuantityCalculated = true;
                }
                context.ForecastOverall.UpdateRange(forecastOveralls);

                updateForecastMaterials.ForEach(x =>
                {
                    x.PriceUnit = null;
                    x.PerUnit = null;
                    x.ForecastOverall = null;
                    x.ForecastDetail = null;

                });


                //var orderDetails = itemStyles.SelectMany(x => x.OrderDetails);

                if (updateForecastMaterials.Any())
                {
                    context.ForecastMaterial.UpdateRange(updateForecastMaterials);
                }

                //if (reservationForecastEntries.Any())
                //{
                //    context.ReservationForecastEntry.AddRange(reservationForecastEntries);
                //}



                //if (updateProductionBOMs.Any())
                //{
                //    context.ProductionBOM.UpdateRange(updateProductionBOMs);
                //}

                try
                {
                    await context.SaveChangesAsync();

                    result.IsSuccess = true;
                    result.Message = "Pull bom sucessfully";
                }
                catch (DbUpdateException exception)
                {
                    result.IsSuccess = false;
                    result.Message = exception.InnerException.Message;
                }
            }
            else
            {
                result.Message = errorMessage;
            }

            return result;
        }
    }
}
