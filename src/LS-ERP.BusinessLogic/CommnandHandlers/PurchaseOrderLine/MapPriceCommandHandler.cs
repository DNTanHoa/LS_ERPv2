using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
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
    public class MapPriceCommandHandler
        : IRequestHandler<MapPriceCommand, MapPriceResult>
    {
        private readonly ILogger logger;
        private readonly IItemPriceRepository itemPriceRepository;
        private readonly AppDbContext context;

        public MapPriceCommandHandler(ILogger<MapPriceCommandHandler> logger,
            IItemPriceRepository itemPriceRepository,
            AppDbContext context)
        {
            this.logger = logger;
            this.itemPriceRepository = itemPriceRepository;
            this.context = context;
        }
        public async Task<MapPriceResult> Handle(
            MapPriceCommand request, CancellationToken cancellationToken)
        {
            var result = new MapPriceResult();
            
            var itemPrices = itemPriceRepository.GetItemPrices(
                request.VendorID, request.ShippingTermCode, request.CustomerID)
                .ToList().OrderByDescending(x => x.EffectDate);

            result.PurchaseOrderLines = new List<PurchaseOrderLine>();

            if(itemPrices.Any())
            {
                foreach(var purchaseOrderLine in request.PurchaseOrderLines)
                {
                    var itemPrice = itemPrices.FirstOrDefault(x => ((x.ItemID == purchaseOrderLine.ItemID && string.IsNullOrEmpty(x.ItemColorCode)) ||
                                                               (string.IsNullOrEmpty(x.ItemID) && x.ItemColorCode == purchaseOrderLine.ItemColorCode) ||
                                                               (x.ItemID == purchaseOrderLine.ItemID && x.ItemColorCode == purchaseOrderLine.ItemColorCode)) &&

                                                               (x.Specify == purchaseOrderLine.Specify ||
                                                                string.IsNullOrEmpty(purchaseOrderLine.Specify)) &&
                                                               (x.VendorID == request.VendorID) &&
                                                               (x.CustomerID == request.CustomerID));
                    if (itemPrice != null)
                    {
                        purchaseOrderLine.Price = itemPrice.Price;
                        var mappedPurchaseOrderLine = purchaseOrderLine;
                        result.PurchaseOrderLines.Add(mappedPurchaseOrderLine);
                    }

                    result.IsSuccess = true;
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "Can't find any price";
            }

            return result;
        }
    }
}
