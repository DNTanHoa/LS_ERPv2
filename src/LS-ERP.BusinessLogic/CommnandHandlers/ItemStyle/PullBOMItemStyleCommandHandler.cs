using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Ressult;
using LS_ERP.BusinessLogic.Events;
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
    public class PullBOMItemStyleCommandHandler
        : IRequestHandler<PullBOMItemStyleCommand, PullBOMItemStyleResult>
    {
        private readonly ILogger<PullBOMItemStyleCommandHandler> logger;
        private readonly IPartRevisionRepository partRevisionRepository;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IWastageSettingRepository wastageSettingRepository;
        private readonly IMediator mediator;
        private readonly SqlServerAppDbContext context;

        public PullBOMItemStyleCommandHandler(ILogger<PullBOMItemStyleCommandHandler> logger,
            IPartRevisionRepository partRevisionRepository,
            IItemStyleRepository itemStyleRepository,
            ICustomerRepository customerRepository,
            IWastageSettingRepository wastageSettingRepository,
            IMediator mediator,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.partRevisionRepository = partRevisionRepository;
            this.itemStyleRepository = itemStyleRepository;
            this.customerRepository = customerRepository;
            this.wastageSettingRepository = wastageSettingRepository;
            this.mediator = mediator;
            this.context = context;
        }

        public async Task<PullBOMItemStyleResult> Handle(PullBOMItemStyleCommand request, CancellationToken cancellationToken)
        {
            var result = new PullBOMItemStyleResult();

            if (!customerRepository.IsExist(request.CustomerID))
            {
                result.Message = "Invalid customer";
                return result;
            }

            logger.LogInformation("{@time} - Exceute pull bom for itemstyle command", DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Exceute pull bom for itemstyle command",
                DateTime.Now.ToString());

            IQueryable<ItemStyle> itemStyles = null;
            IQueryable<PartRevision> partRevisions = null;

            List<ProductionBOM> newProductionBOMs = null;
            List<ProductionBOM> updateProductionBOMs = null;
            List<JobHead> jobHeads = null;
            List<ReservationEntry> reservationEntries = null;
            List<WastageSetting> wastageSettings = null;
            string errorMessage = string.Empty;

            bool pullBomResult = false;

            switch (request.CustomerID)
            {
                case "HA":
                case "PU":
                    itemStyles = itemStyleRepository.GetItemStyles(request.ItemStyleNumbers.ToList());
                    partRevisions = partRevisionRepository.GetLastestPartRevisions(itemStyles.Select(x => x.ContractNo)
                        .ToList());
                    wastageSettings = wastageSettingRepository.GetWastageSettings(request.CustomerID)
                        .ToList();

                    pullBomResult = PullBomItemStyleProcess.PullForCustomerHA(itemStyles.ToList(), partRevisions.ToList(), request.Username,
                        wastageSettings, out newProductionBOMs, out updateProductionBOMs,
                        out jobHeads, out reservationEntries, out errorMessage);
                    break;
                case "IFG":

                    itemStyles = itemStyleRepository.GetItemStyles(request.ItemStyleNumbers.ToList());
                    partRevisions = partRevisionRepository.GetLastestPartRevisions(itemStyles.Select(x => x.ContractNo)
                        .ToList());
                    wastageSettings = wastageSettingRepository.GetWastageSettings(request.CustomerID)
                        .ToList();

                    pullBomResult = PullBomItemStyleProcess.PullForCustomerIFG(itemStyles.ToList(), partRevisions.ToList(), request.Username,
                        wastageSettings, out newProductionBOMs, out updateProductionBOMs,
                        out jobHeads, out reservationEntries, out errorMessage);
                    break;
                case "DE":
                    itemStyles = itemStyleRepository.GetItemStyles(request.ItemStyleNumbers.ToList());
                    partRevisions = partRevisionRepository.GetLastestPartRevisions(itemStyles.Select(x => x.CustomerStyle).ToList());

                    pullBomResult = PullBomItemStyleProcess.PullForCustomerDE(itemStyles.ToList(), partRevisions.ToList(), request.Username,
                        out newProductionBOMs, out updateProductionBOMs,
                        out jobHeads, out reservationEntries, out errorMessage);
                    break;
                default:
                    itemStyles = itemStyleRepository.GetItemStyles(request.ItemStyleNumbers.ToList());
                    partRevisions = partRevisionRepository.GetLastestPartRevisions(itemStyles.Select(x => x.CustomerStyle).ToList());
                    wastageSettings = wastageSettingRepository.GetWastageSettings(request.CustomerID)
                       .ToList();

                    pullBomResult = PullBomItemStyleProcess.PullDefault(itemStyles.ToList(), partRevisions.ToList(), request.Username,
                        wastageSettings,
                        out newProductionBOMs, out updateProductionBOMs,
                        out jobHeads, out reservationEntries, out errorMessage);
                    break;
            }

            result.IsSuccess = pullBomResult;

            if (pullBomResult)
            {

                foreach (var itemStyle in itemStyles)
                {
                    itemStyle.IsBomPulled = true;
                    itemStyle.IsCalculateRequiredQuantity = true;
                }

                newProductionBOMs.ForEach(x =>
                {
                    x.PriceUnit = null;
                    x.PerUnit = null;
                    x.ItemStyle = null;
                    x.JobHead = null;
                });

                updateProductionBOMs.ForEach(x =>
                {
                    x.PriceUnit = null;
                    x.PerUnit = null;
                    x.ItemStyle = null;
                    x.JobHead = null;
                });

                var orderDetails = itemStyles.SelectMany(x => x.OrderDetails);

                if (jobHeads.Any())
                {
                    context.JobHead.AddRange(jobHeads);
                    context.OrderDetail.UpdateRange(itemStyles.ToList().SelectMany(x => x.OrderDetails));
                }

                if (reservationEntries.Any())
                {
                    context.ReservationEntry.AddRange(reservationEntries);
                }

                if (newProductionBOMs.Any())
                {
                    context.ProductionBOM.AddRange(newProductionBOMs);
                }

                if (updateProductionBOMs.Any())
                {
                    context.ProductionBOM.UpdateRange(updateProductionBOMs);
                }

                try
                {
                    await context.SaveChangesAsync();

                    result.IsSuccess = true;
                    result.Message = "Pull bom sucessfully";

                    await mediator.Publish(new ItemStylePullBomEvent()
                    {
                        ItemStyleNumbers = request.ItemStyleNumbers.ToList()
                    }).ConfigureAwait(false);
                }
                catch (DbUpdateException exception)
                {
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
