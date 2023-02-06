using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
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
    public class GroupToPurchaseOrderLineCommandHandler
        : IRequestHandler<GroupToPurchaseOrderLineCommand, GroupToPurchaseOrderLineResult>
    {
        private readonly ILogger<GroupToPurchaseOrderLineCommandHandler> logger;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;

        public GroupToPurchaseOrderLineCommandHandler(
            ILogger<GroupToPurchaseOrderLineCommandHandler> logger,
            IMapper mapper,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<GroupToPurchaseOrderLineResult> Handle(GroupToPurchaseOrderLineCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute group production Boms to purchase order line command", 
                DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Exceute group production Boms to purchase order line command",
                DateTime.Now.ToString());

            var result = new GroupToPurchaseOrderLineResult();

            foreach(var productionBOM in request.ProductionBOMs)
            {
                var purchaseOrderLine = request.PurchaseOrderLines?
                    .FirstOrDefault(x => x.ItemID == productionBOM.ItemID &&
                                         x.ItemName == productionBOM.ItemName &&
                                         x.ItemColorCode == productionBOM.ItemColorCode &&
                                         x.ItemColorName == productionBOM.ItemColorName &&
                                         (x.Specify == productionBOM.Specify || string.IsNullOrEmpty(x.Specify))&&
                                         (x.Position == productionBOM.Position || string.IsNullOrEmpty(x.Position)) &&
                                         x.LSStyle == productionBOM.ItemStyle?.LSStyle &&
                                         x.SalesOrderID == productionBOM.ItemStyle?.SalesOrderID &&
                                         x.GarmentColorCode == productionBOM.ItemStyle?.ColorCode &&
                                         x.GarmentColorName == productionBOM.ItemStyle?.ColorName &&
                                         (x.ContractNo == productionBOM.ItemStyle?.ContractNo || string.IsNullOrEmpty(x.ContractNo))&&
                                         (x.GarmentSize == productionBOM.GarmentSize || string.IsNullOrEmpty(x.GarmentSize)) &&
                                         x.ID > 0);

                if(purchaseOrderLine != null)
                {
                    purchaseOrderLine.Quantity += productionBOM.RequiredQuantity;
                    purchaseOrderLine.ReservedQuantity += productionBOM.RequiredQuantity;
                    purchaseOrderLine.WastageQuantity += productionBOM.WastageQuantity;
                    purchaseOrderLine.WareHouseQuantity += productionBOM.WareHouseQuantity;

                    var reservation = new ReservationEntry()
                    {
                        ProductionBOMID = productionBOM.ID,
                        ReservedQuantity = productionBOM.RequiredQuantity,
                    };

                    purchaseOrderLine.ReservationEntries.Add(reservation);
                }
                else
                {
                    var newPurchaseOrderLine = new PurchaseOrderLine()
                    {
                        PurchaseOrderID = request.PurchaserOrderID,
                        Quantity = productionBOM.RequiredQuantity,
                        ReservedQuantity = productionBOM.ReservedQuantity,
                        WastageQuantity = productionBOM.WastageQuantity,
                        WareHouseQuantity = productionBOM.WareHouseQuantity,
                    };
                    mapper.Map(productionBOM, newPurchaseOrderLine);

                    if (request.PurchaseOrderLines == null)
                        request.PurchaseOrderLines = new List<PurchaseOrderLine>();

                    var reservation = new ReservationEntry()
                    {
                        ProductionBOMID = productionBOM.ID,
                        ReservedQuantity = productionBOM.RequiredQuantity,
                    };

                    newPurchaseOrderLine.ReservationEntries.Add(reservation);
                    request.PurchaseOrderLines.Add(newPurchaseOrderLine);
                }
            }

            if (request.PurchaseOrderLines != null)
            {
                var newLines = request.PurchaseOrderLines.Where(x => x.ID == 0).ToList();
                foreach (var purchaseOrderLine in newLines)
                {
                    var groupLine = request.PurchaseOrderGroupLines?
                        .FirstOrDefault(x => x.ItemID == purchaseOrderLine.ItemID &&
                                             x.ItemName == purchaseOrderLine.ItemName &&
                                             x.ItemColorCode == purchaseOrderLine.ItemColorCode &&
                                             x.ItemColorName == purchaseOrderLine.ItemColorName &&
                                             (x.Specify == purchaseOrderLine.Specify || string.IsNullOrEmpty(x.Specify)) &&
                                             (x.Position == purchaseOrderLine.Position || string.IsNullOrEmpty(x.Position)) &&
                                             x.CustomerStyle == purchaseOrderLine.CustomerStyle &&
                                             x.GarmentColorName == purchaseOrderLine.GarmentColorName &&
                                             x.GarmentColorCode == purchaseOrderLine.GarmentColorCode &&
                                             (x.ContractNo == purchaseOrderLine.ContractNo || string.IsNullOrEmpty(x.ContractNo)) &&
                                             (x.GarmentSize == purchaseOrderLine.GarmentSize || string.IsNullOrEmpty(x.GarmentSize)));
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
