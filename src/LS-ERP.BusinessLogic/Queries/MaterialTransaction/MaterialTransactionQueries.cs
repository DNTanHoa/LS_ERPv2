using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class MaterialTransactionQueries : IMaterialTransactionQueries
    {
        private readonly AppDbContext context;

        public MaterialTransactionQueries(AppDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<MaterialTransactionSummaryDto> GetSummaryDtos(
            string storageCode, DateTime fromDate, DateTime toDate)
        {
            var materialTransactions = context.MaterialTransaction
                .Include(x => x.Receipt)
                .Include(x => x.Issued)
                .Where(x => x.StorageCode == storageCode &&
                            ((DateTime)x.TransactionDate).Date >= fromDate &&
                            ((DateTime)x.TransactionDate).Date  <= toDate);

            var summaryTransactions = materialTransactions
                .GroupBy(x => new
                {
                    x.ItemCode,
                    x.ItemID,
                    x.ItemName,
                    x.ItemColorCode,
                    x.ItemColorName,
                    x.Position,
                    x.Specify,
                    x.Season,
                    x.GarmentColorCode,
                    x.GarmentColorName,
                    x.GarmentSize,
                    x.UnitID,
                    x.CustomerStyle,
                    x.LSStyle,
                    CustomerID = x.Receipt.CustomerID ?? x.Issued.CustomerID
                })
                .Select(g => new MaterialTransactionSummaryDto 
                {
                    ItemCode = g.Key.ItemCode,
                    ItemID = g.Key.ItemID,
                    ItemName = g.Key.ItemName,
                    ItemColorCode = g.Key.ItemColorCode,
                    ItemColorName = g.Key.ItemColorName,
                    Position = g.Key.Position,
                    Specify = g.Key.Specify,
                    Season = g.Key.Season,
                    GarmentColorCode = g.Key.GarmentColorCode,
                    GarmentColorName = g.Key.GarmentColorName,
                    GarmentSize = g.Key.GarmentSize,
                    UnitID = g.Key.UnitID,
                    CustomerStyle = g.Key.CustomerStyle,
                    LSStyle = g.Key.LSStyle,
                    CustomerID = g.Key.CustomerID,
                    InQuantity = g.Where(x => x.Quantity > 0).Sum(x => x.Quantity ?? 0),
                    OutQuantity = g.Where(x => x.Quantity < 0).Sum(x => x.Quantity ?? 0)
                });

            return summaryTransactions;
        }
    }
}
