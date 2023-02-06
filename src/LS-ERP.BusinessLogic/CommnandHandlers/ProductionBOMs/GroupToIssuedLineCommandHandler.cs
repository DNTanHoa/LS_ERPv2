using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
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
    public class GroupToIssuedLineCommandHandler
        : IRequestHandler<GroupToIssuedLineCommand, GroupToIssuedLineResult>
    {
        private readonly ILogger<GroupToIssuedLineCommandHandler> logger;
        private readonly IMapper mapper;

        public GroupToIssuedLineCommandHandler(ILogger<GroupToIssuedLineCommandHandler> logger,
            IMapper mapper)
        {
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<GroupToIssuedLineResult> Handle(GroupToIssuedLineCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute group production Boms to issued line command",
                DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Exceute group production Boms to issued line command",
                DateTime.Now.ToString());

            var result = new GroupToIssuedLineResult();

            foreach(var productionBOM in request.ProductionBOMs)
            {
                var issuedLine = request.IssuedLines?
                    .FirstOrDefault(x => x.ItemID == productionBOM.ItemID &&
                                         x.ItemName == productionBOM.ItemName &&
                                         x.ItemColorCode == productionBOM.ItemColorCode &&
                                         x.ItemColorName == productionBOM.ItemColorName &&
                                         x.Specify == productionBOM.Specify &&
                                         x.Position == productionBOM.Position &&
                                         x.LSStyle == productionBOM.ItemStyle?.LSStyle &&
                                         x.GarmentColorCode == productionBOM.ItemStyle?.ColorCode &&
                                         x.GarmentColorName == productionBOM.ItemStyle?.ColorName);

                if(issuedLine != null)
                {
                    issuedLine.IssuedQuantity += productionBOM.RequiredQuantity;
                }
                else
                {
                    issuedLine = new IssuedLine()
                    {
                        ItemCode = productionBOM.ItemCode,
                        ItemID = productionBOM.ItemID,
                        ItemName = productionBOM.ItemName,
                        ItemColorCode = productionBOM.ItemColorCode,
                        ItemColorName = productionBOM.ItemColorName,
                        Position = productionBOM.Position,
                        Specify = productionBOM.Specify,
                        UnitID = productionBOM.PerUnitID,
                        Season = productionBOM.ItemStyle?.Season,
                        GarmentColorCode = productionBOM.ItemStyle?.ColorCode,
                        GarmentColorName = productionBOM.ItemStyle?.ColorName,
                        GarmentSize = productionBOM.GarmentSize,
                        CustomerStyle = productionBOM.ItemStyle?.CustomerStyle,
                        LSStyle = productionBOM.ItemStyle?.LSStyle,

                        ProductionBOMID = productionBOM.ID,
                        IssuedQuantity = productionBOM.ConsumptionQuantity
                    };

                    if (request.IssuedLines == null)
                    {
                        request.IssuedLines = new List<IssuedLine>();
                    }

                    request.IssuedLines.Add(issuedLine);
                }
            }

            if(request.IssuedLines != null)
            {
                foreach (var issuedLine in request.IssuedLines)
                {
                    var groupLine = request.IssuedGroupLines?
                        .FirstOrDefault(x => x.ItemID == issuedLine.ItemID &&
                                             x.ItemName == issuedLine.ItemName &&
                                             x.ItemColorCode == issuedLine.ItemColorCode &&
                                             x.ItemColorName == issuedLine.ItemColorName &&
                                             x.Specify == issuedLine.Specify &&
                                             x.Position == issuedLine.Position &&
                                             x.CustomerStyle == issuedLine.CustomerStyle &&
                                             x.GarmentColorName == issuedLine.GarmentColorName &&
                                             x.GarmentColorCode == issuedLine.GarmentColorCode);

                    if(groupLine != null)
                    {
                        groupLine.IssuedQuantity += issuedLine.IssuedQuantity;
                        groupLine.IssuedLines.Add(issuedLine);
                    }
                    else
                    {
                        groupLine = new IssuedGroupLine()
                        {
                            ItemCode = issuedLine.ItemCode,
                            ItemID = issuedLine.ItemID,
                            ItemName = issuedLine.ItemName,
                            ItemColorCode = issuedLine.ItemColorCode,
                            ItemColorName = issuedLine.ItemColorName,
                            Position = issuedLine.Position,
                            Specify = issuedLine.Specify,
                            Season = issuedLine.Season,
                            UnitID = issuedLine.UnitID,
                            GarmentColorCode = issuedLine.GarmentColorCode,
                            GarmentColorName = issuedLine.GarmentColorName,
                            CustomerStyle = issuedLine.CustomerStyle,
                            IssuedQuantity = issuedLine.IssuedQuantity,
                            IssuedNumber = request.IssuedNumber,
                        };

                        if (request.IssuedGroupLines == null)
                            request.IssuedGroupLines = new List<IssuedGroupLine>();

                        groupLine.IssuedLines.Add(issuedLine);
                        request.IssuedGroupLines.Add(groupLine);
                    }
                }
            }

            result.IssuedGroupLines = request.IssuedGroupLines;
            result.IssuedLines = request.IssuedLines;

            result.IsSuccess = true;

            return result;
        }
    }
}
