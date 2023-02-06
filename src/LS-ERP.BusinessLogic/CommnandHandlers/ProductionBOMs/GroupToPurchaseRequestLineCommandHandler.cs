using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class GroupToPurchaseRequestLineCommandHandler
        : IRequestHandler<GroupToPurchaseRequestLineCommand, GroupToPurchaseRequestLineResult>
    {
        private readonly ILogger<GroupToPurchaseRequestLineCommandHandler> logger;
        private readonly IMapper mapper;

        public GroupToPurchaseRequestLineCommandHandler(
            ILogger<GroupToPurchaseRequestLineCommandHandler> logger,
            IMapper mapper)
        {
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<GroupToPurchaseRequestLineResult> Handle(GroupToPurchaseRequestLineCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute group production Boms to purchase request line command",
                DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Exceute group production Boms to purchase request line command",
                DateTime.Now.ToString());

            var result = new GroupToPurchaseRequestLineResult();

            foreach (var productionBOM in request.ProductionBOMs)
            {
                var PurchaseRequestLine = request.PurchaseRequestLines?
                    .FirstOrDefault(x => x.ItemID == productionBOM.ItemID &&
                                         x.ItemName == productionBOM.ItemName &&
                                         x.ItemColorCode == productionBOM.ItemColorCode &&
                                         x.ItemColorName == productionBOM.ItemColorName &&
                                         x.Specify == productionBOM.Specify &&
                                         x.Position == productionBOM.Position &&
                                         x.LSStyle == productionBOM.ItemStyle?.LSStyle &&
                                         x.GarmentColorCode == productionBOM.ItemStyle?.ColorCode &&
                                         x.GarmentColorName == productionBOM.ItemStyle?.ColorName &&
                                         x.ContractNo == productionBOM.ItemStyle?.ContractNo &&
                                         x.GarmentSize == productionBOM.GarmentSize);

                if (PurchaseRequestLine != null)
                {
                    PurchaseRequestLine.Quantity += productionBOM.RequiredQuantity;
                }
                else
                {
                    PurchaseRequestLine = new PurchaseRequestLine()
                    {
                        PurchaseRequestID = request.PurchaserRequestID,
                        Quantity = productionBOM.RequiredQuantity,
                        RemainQuantity = productionBOM.RequiredQuantity,
                    };
                    mapper.Map(productionBOM, PurchaseRequestLine);

                    if (request.PurchaseRequestLines == null)
                        request.PurchaseRequestLines = new List<PurchaseRequestLine>();

                    request.PurchaseRequestLines.Add(PurchaseRequestLine);
                }
            }

            if (request.PurchaseRequestLines != null)
            {
                foreach (var PurchaseRequestLine in request.PurchaseRequestLines)
                {
                    var groupLine = request.PurchaseRequestGroupLines?
                        .FirstOrDefault(x => x.ItemID == PurchaseRequestLine.ItemID &&
                                             x.ItemName == PurchaseRequestLine.ItemName &&
                                             x.ItemColorCode == PurchaseRequestLine.ItemColorCode &&
                                             x.ItemColorName == PurchaseRequestLine.ItemColorName &&
                                             x.Specify == PurchaseRequestLine.Specify &&
                                             x.Position == PurchaseRequestLine.Position &&
                                             x.CustomerStyle == PurchaseRequestLine.CustomerStyle &&
                                             x.GarmentColorName == PurchaseRequestLine.GarmentColorName &&
                                             x.GarmentColorCode == PurchaseRequestLine.GarmentColorCode &&
                                             x.ContractNo == PurchaseRequestLine.ContractNo &&
                                             x.GarmentSize == PurchaseRequestLine.GarmentSize);
                    if (groupLine != null)
                    {
                        groupLine.Quantity += PurchaseRequestLine.Quantity;

                        if (request.PurchaseRequestGroupLines == null)
                            request.PurchaseRequestGroupLines = new List<PurchaseRequestGroupLine>();
                    }
                    else
                    {
                        groupLine = mapper.Map<PurchaseRequestGroupLine>(PurchaseRequestLine);

                        if (request.PurchaseRequestGroupLines == null)
                            request.PurchaseRequestGroupLines = new List<PurchaseRequestGroupLine>();

                        request.PurchaseRequestGroupLines.Add(groupLine);
                    }

                    groupLine.PurchaseRequestLines.Add(PurchaseRequestLine);
                }
            }

            result.IsSuccess = true;
            result.PurchaseRequestLines = request.PurchaseRequestLines;
            result.PurchaseRequestGroupLines = request.PurchaseRequestGroupLines;

            return result;
        }
    }
}
