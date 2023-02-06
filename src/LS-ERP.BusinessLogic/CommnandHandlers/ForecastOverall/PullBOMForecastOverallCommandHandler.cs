using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
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
    public class PullBOMForecastOverallCommandHandler
        : IRequestHandler<PullBOMForecastOverallCommand, PullBOMForecastOverallResult>
    {
        private readonly ILogger<PullBOMForecastOverallCommandHandler> logger;
        private readonly IPartRevisionRepository partRevisionRepository;
        private readonly IForecastOverallRepository forecastOverallRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IWastageSettingRepository wastageSettingRepository;
        private readonly SqlServerAppDbContext context;

        public PullBOMForecastOverallCommandHandler(ILogger<PullBOMForecastOverallCommandHandler> logger,
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

        public async Task<PullBOMForecastOverallResult> Handle(PullBOMForecastOverallCommand request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute pull bom for forecast command", DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Exceute pull bom for forecast command",
                DateTime.Now.ToString());

            var result = new PullBOMForecastOverallResult();

            if (!customerRepository.IsExist(request.CustomerID))
            {
                result.Message = "Invalid customer";
                return result;
            }

            IQueryable<ForecastOverall> forecastOveralls = null;
            IQueryable<PartRevision> partRevisions = null;

            List<ForecastMaterial> newForecastMaterials = null;
            List<ForecastMaterial> updateForecastMaterials = null;
            List<WastageSetting> wastageSettings = null;
            string errorMessage = string.Empty;

            bool pullBomResult = false;

            switch (request.CustomerID)
            {
                case "GA":
                    {
                        forecastOveralls = forecastOverallRepository.GetForecastOveralls(request.ForecastOverallIDs.ToList());
                        partRevisions = partRevisionRepository.GetLastestPartRevisions(forecastOveralls.Select(x => x.CustomerStyle)
                            .ToList());
                        wastageSettings = wastageSettingRepository.GetWastageSettings(request.CustomerID)
                            .ToList();

                        pullBomResult = PullBOMForecastOverallProcess.PullBOMGaran(forecastOveralls.ToList(),
                            request.Username, partRevisions.ToList(),
                            wastageSettings, out newForecastMaterials,
                            out updateForecastMaterials,
                            out errorMessage);
                    }
                    break;
                case "DE":
                    {
                        forecastOveralls = forecastOverallRepository.GetForecastOveralls(request.ForecastOverallIDs.ToList());
                        partRevisions = partRevisionRepository.GetLastestPartRevisions(forecastOveralls.Select(x => x.CustomerStyle)
                            .ToList());
                        wastageSettings = wastageSettingRepository.GetWastageSettings(request.CustomerID)
                            .ToList();

                        pullBomResult = PullBOMForecastOverallProcess.PullBOM(forecastOveralls.ToList(),
                            request.Username, partRevisions.ToList(),
                            wastageSettings, out newForecastMaterials, out updateForecastMaterials,
                            out errorMessage);
                    }
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

                newForecastMaterials.ForEach(x =>
                {
                    x.PriceUnit = null;
                    x.PerUnit = null;
                    x.ForecastOverall = null;
                    x.ForecastDetail = null;
                    //x.ReservationForecastEntries = null;

                });

                //var orderDetails = itemStyles.SelectMany(x => x.OrderDetails);

                if (newForecastMaterials.Any())
                {
                    context.ForecastMaterial.AddRange(newForecastMaterials);
                }

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
