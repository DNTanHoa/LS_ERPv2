using AutoMapper;
using Castle.Core.Logging;
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
    public class GroupForecastMaterialToPurchaseOrderLineCommandHandler
        : IRequestHandler<GroupForecastMaterialToPurchaseOrderLineCommand,
            GroupForecastMaterialToPurchaseOrderLineResult>
    {
        private readonly ILogger<GroupForecastMaterialToPurchaseOrderLineCommandHandler> logger;
        private readonly IMapper mapper;

        public GroupForecastMaterialToPurchaseOrderLineCommandHandler(
            ILogger<GroupForecastMaterialToPurchaseOrderLineCommandHandler> logger,
            IMapper mapper)
        {
            this.logger = logger;
            this.mapper = mapper;
        }
        public async Task<GroupForecastMaterialToPurchaseOrderLineResult> Handle(
            GroupForecastMaterialToPurchaseOrderLineCommand request, 
            CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute group forecast material to purchase order line command",
                DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Exceute group forecast material to purchase order line command",
                DateTime.Now.ToString());
            var result = new GroupForecastMaterialToPurchaseOrderLineResult();

            foreach (var forecastMaterial in request.ForecastMaterials)
            {
                var purchaseOrderLine = request.PurchaseOrderLines?
                    .FirstOrDefault(x => x.ItemID == forecastMaterial.ItemID &&
                                         x.ItemName == forecastMaterial.ItemName &&
                                         x.ItemColorCode == forecastMaterial.ItemColorCode &&
                                         x.ItemColorName == forecastMaterial.ItemColorName &&
                                         x.Specify == forecastMaterial.Specify &&
                                         x.Position == forecastMaterial.Position &&
                                         x.GarmentColorCode == forecastMaterial.ForecastOverall?.GarmentColorCode &&
                                         x.GarmentColorName == forecastMaterial.ForecastOverall?.GarmentColorName &&
                                         x.ContractNo == forecastMaterial.ForecastOverall?.ContractNo &&
                                         x.GarmentSize == forecastMaterial.GarmentSize &&
                                         x.ID > 0);

                if (purchaseOrderLine != null)
                {
                    purchaseOrderLine.Quantity += forecastMaterial.RequiredQuantity;
                    purchaseOrderLine.ReservedQuantity += forecastMaterial.RequiredQuantity;
                    purchaseOrderLine.WastageQuantity += forecastMaterial.WastageQuantity;
                    purchaseOrderLine.WareHouseQuantity += forecastMaterial.WareHouseQuantity;

                    var reservation = new ReservationForecastEntry()
                    {
                        ForecastMaterialID = forecastMaterial.ID,
                        ReservedQuantity = forecastMaterial.RequiredQuantity,
                    };

                    purchaseOrderLine.ReservationForecastEntries.Add(reservation);
                }
                else
                {
                    var newPurchaseOrderLine = new PurchaseOrderLine()
                    {
                        PurchaseOrderID = request.PurchaserOrderID,
                        Quantity = forecastMaterial.RequiredQuantity,
                        ReservedQuantity = forecastMaterial.ReservedQuantity,
                        WastageQuantity = forecastMaterial.WastageQuantity,
                        WareHouseQuantity = forecastMaterial.WareHouseQuantity,
                    };
                    mapper.Map(forecastMaterial, newPurchaseOrderLine);

                    if (request.PurchaseOrderLines == null)
                        request.PurchaseOrderLines = new List<PurchaseOrderLine>();

                    var reservation = new ReservationForecastEntry()
                    {
                        ForecastMaterialID = forecastMaterial.ID,
                        ReservedQuantity = forecastMaterial.RequiredQuantity,
                    };

                    newPurchaseOrderLine.ReservationForecastEntries.Add(reservation);
                    request.PurchaseOrderLines.Add(newPurchaseOrderLine);
                }
            }

            if(request.PurchaseOrderLines != null)
            {
                var newLines = request.PurchaseOrderLines.Where(x => x.ID == 0).ToList();
                foreach (var purchaseOrderLine in newLines)
                {
                    var groupLine = request.PurchaseOrderGroupLines?
                        .FirstOrDefault(x => x.ItemID == purchaseOrderLine.ItemID &&
                                             x.ItemName == purchaseOrderLine.ItemName &&
                                             x.ItemColorCode == purchaseOrderLine.ItemColorCode &&
                                             x.ItemColorName == purchaseOrderLine.ItemColorName &&
                                             x.Specify == purchaseOrderLine.Specify &&
                                             x.Position == purchaseOrderLine.Position &&
                                             x.CustomerStyle == purchaseOrderLine.CustomerStyle &&
                                             x.GarmentColorName == purchaseOrderLine.GarmentColorName &&
                                             x.GarmentColorCode == purchaseOrderLine.GarmentColorCode &&
                                             x.ContractNo == purchaseOrderLine.ContractNo &&
                                             x.GarmentSize == purchaseOrderLine.GarmentSize);
                    if (groupLine != null)
                    {
                        groupLine.Quantity += purchaseOrderLine.Quantity;
                        groupLine.WastageQuantity += purchaseOrderLine.WastageQuantity;
                        groupLine.WareHouseQuantity += purchaseOrderLine.WareHouseQuantity;

                        groupLine.PurchaseOrderLines.Add(purchaseOrderLine);
                    }
                    else
                    {
                        var newGroupLine = mapper.Map<PurchaseOrderGroupLine>(purchaseOrderLine);
                        newGroupLine.WareHouseUnitID = purchaseOrderLine.SecondUnitID;

                        if (request.PurchaseOrderGroupLines == null)
                            request.PurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();

                        newGroupLine.PurchaseOrderLines.Add(purchaseOrderLine);
                        request.PurchaseOrderGroupLines.Add(newGroupLine);
                    }
                }
            }

            result.IsSuccess = true;
            result.PurchaseOrderLines = request.PurchaseOrderLines;
            result.PurchaseOrderGroupLines = request.PurchaseOrderGroupLines;

            return result;
        }
    }
}
