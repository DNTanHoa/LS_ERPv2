using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
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
    public class GroupPurchaseRequestToPurchaseOrderLineCommandHandler
        : IRequestHandler<GroupPurchaseRequestToPurchaseOrderLineCommand, GroupPurchaseRequestToPurchaseOrderLineResult>
    {
        private readonly ILogger<GroupPurchaseRequestToPurchaseOrderLineCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public GroupPurchaseRequestToPurchaseOrderLineCommandHandler(
            ILogger<GroupPurchaseRequestToPurchaseOrderLineCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<GroupPurchaseRequestToPurchaseOrderLineResult> Handle(
            GroupPurchaseRequestToPurchaseOrderLineCommand request, CancellationToken cancellationToken)
        {
            var result = new GroupPurchaseRequestToPurchaseOrderLineResult();
            
            try
            {
                foreach (var purchaseRequestLine in request.PurchaseRequestLines)
                {
                    var purchaseOrderLine = request.PurchaseOrderLines?
                        .FirstOrDefault(x => x.ItemID == purchaseRequestLine.ItemID &&
                                             x.ItemName == purchaseRequestLine.ItemName &&
                                             x.ItemColorCode == purchaseRequestLine.ItemColorCode &&
                                             x.ItemColorName == purchaseRequestLine.ItemColorName &&
                                             x.Specify == purchaseRequestLine.Specify &&
                                             x.Position == purchaseRequestLine.Position &&
                                             x.LSStyle == purchaseRequestLine.LSStyle &&
                                             x.PurchaseRequestID == purchaseRequestLine.PurchaseRequestID &&
                                             x.GarmentColorCode == purchaseRequestLine.GarmentColorCode &&
                                             x.GarmentColorName == purchaseRequestLine.GarmentColorName &&
                                             x.GarmentSize == purchaseRequestLine.GarmentSize &&
                                             x.ID > 0);

                    if (purchaseOrderLine != null)
                    {
                        purchaseOrderLine.Quantity += purchaseRequestLine.Quantity;
                        purchaseOrderLine.ReservedQuantity += purchaseRequestLine.Quantity;
                        purchaseOrderLine.WareHouseQuantity += purchaseRequestLine.Quantity;

                        var reservation = new ReservationEntry()
                        {
                            PurchaseRequestLineID = purchaseRequestLine.ID,
                            ReservedQuantity = purchaseRequestLine.Quantity,
                        };

                        purchaseOrderLine.ReservationEntries.Add(reservation);
                    }
                    else
                    {
                        
                        var newPurchaseOrderLine = mapper.Map<PurchaseOrderLine>(purchaseRequestLine);
                        newPurchaseOrderLine.PurchaseOrderID = request.PurchaserOrderID;
                        newPurchaseOrderLine.Quantity = purchaseRequestLine.Quantity;
                        newPurchaseOrderLine.ReservedQuantity = purchaseRequestLine.Quantity;
                        newPurchaseOrderLine.WastageQuantity = purchaseRequestLine.Quantity;
                        newPurchaseOrderLine.WareHouseQuantity = purchaseRequestLine.Quantity;
                       
                        if (request.PurchaseOrderLines == null)
                            request.PurchaseOrderLines = new List<PurchaseOrderLine>();

                        var reservation = new ReservationEntry()
                        {
                            PurchaseRequestLineID = newPurchaseOrderLine.ID,
                            ReservedQuantity = newPurchaseOrderLine.Quantity,
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
            }
            catch(Exception ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Execute log purchase request to purchase order command {@message}", 
                    DateTime.Now.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Execute log purchase request to purchase order command",
                    DateTime.Now.ToString(), ex.InnerException.Message);
            }

            return result;
        }
    }
}
