using Hangfire;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class UpdateShipQuantityJob
    {
        private readonly ILogger<UpdateShipQuantityJob> logger;
        private readonly SqlServerAppDbContext context;

        public UpdateShipQuantityJob(ILogger<UpdateShipQuantityJob> logger,
            SqlServerAppDbContext context) 
        {
            this.logger = logger;
            this.context = context;
        }

        [JobDisplayName("Update ship quantity & create item style bar code")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute(List<string> ItemStyleNumbers)
        {
            var orderDetails = context.OrderDetail
                .Where(x => ItemStyleNumbers.Contains(x.ItemStyleNumber)).ToList();

            var ItemStyles = context.ItemStyle
                .Include(x => x.SalesOrder)
                .Where(x => ItemStyleNumbers.Contains(x.Number)).ToList();

            var ItemStyleBarCodes = new List<ItemStyleBarCode>();
            var SortItemStyleBarCodes = new List<ItemStyleBarCode>();

            /// Set ShipQuantity = Quantity
            if (orderDetails != null && orderDetails.Any())
            {
                orderDetails.ForEach(x =>
                {
                    x.ShipQuantity = x.Quantity;
                });
            }

            /// Create ItemStyleBarCode for IFG
            if (ItemStyles.FirstOrDefault()?.SalesOrder?.CustomerID.Trim().ToUpper() == "IFG")
            {
                orderDetails.ForEach(x =>
                {
                    var barCode = context.ItemStyleBarCode
                        .FirstOrDefault(b => b.ItemStyleNumber == x.ItemStyleNumber && b.Size == x.Size);
                    if(barCode == null)
                    {
                        var ItemStyleBarCode = new ItemStyleBarCode()
                        {
                            ItemStyleNumber = x.ItemStyleNumber,
                            Size = x.Size,
                            SizeSortIndex = x.SizeSortIndex,
                        };
                        ItemStyleBarCodes.Add(ItemStyleBarCode);
                    }
                });
            }

            /// Set Multi ship = true for IFG & GA
            /// Set PSDD = Contract Date for DE
            if (ItemStyles.FirstOrDefault()?.SalesOrder?.CustomerID.Trim().ToUpper() == "IFG" ||
                ItemStyles.FirstOrDefault()?.SalesOrder?.CustomerID.Trim().ToUpper() == "GA" ||
                ItemStyles.FirstOrDefault()?.SalesOrder?.CustomerID.Trim().ToUpper() == "DE")
            {
                ItemStyles.ForEach(x =>
                {
                    if (ItemStyles.FirstOrDefault()?.SalesOrder?.CustomerID.Trim().ToUpper() == "IFG" ||
                            ItemStyles.FirstOrDefault()?.SalesOrder?.CustomerID.Trim().ToUpper() == "GA")
                    {
                        x.MultiShip = true;
                    }
                    else if(ItemStyles.FirstOrDefault()?.SalesOrder?.CustomerID.Trim().ToUpper() == "DE")
                    {
                        x.MultiShip = true;
                        x.ProductionSkedDeliveryDate = x.EstimatedSupplierHandOver;
                    }
                });
            }

            /// Set Size Sort Index for Item Style Bar Code
            orderDetails.ForEach(x =>
            {
                if(ItemStyles.FirstOrDefault()?.SalesOrder?.CustomerID.Trim().ToUpper() == "DE")
                {
                    var barCode = context.ItemStyleBarCode
                        .FirstOrDefault(b => b.ItemStyleNumber == x.ItemStyleNumber 
                                        && b.Size.Trim().Replace(" ","").ToUpper() == x.Size.Trim().Replace(" ", "").ToUpper());
                    if (barCode != null)
                    {
                        barCode.SizeSortIndex = x.SizeSortIndex;
                        SortItemStyleBarCodes.Add(barCode);
                    }
                }
                else
                {
                    var barCode = context.ItemStyleBarCode
                        .FirstOrDefault(b => b.ItemStyleNumber == x.ItemStyleNumber && b.Size == x.Size);
                    if (barCode != null)
                    {
                        barCode.SizeSortIndex = x.SizeSortIndex;
                        SortItemStyleBarCodes.Add(barCode);
                    }
                }
            });

            /// Update handle
            try
            {
                context.OrderDetail.UpdateRange(orderDetails);
                context.ItemStyleBarCode.AddRange(ItemStyleBarCodes);
                context.ItemStyle.UpdateRange(ItemStyles);
                context.ItemStyleBarCode.UpdateRange(SortItemStyleBarCodes);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("Sales order event handler has error with message {@message}",
                    ex.InnerException?.Message);
                LogHelper.Instance.Error("Sales order event handler has error with message {@message}",
                    ex.InnerException?.Message);
            }

            return Task.CompletedTask;  
        }
    }
}
