using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class ItemStyleQueries : IItemStyleQueries
    {
        private readonly SqlServerAppDbContext context;

        public ItemStyleQueries(SqlServerAppDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<MaterialDtos> GetMaterials(string styles)
        {
            var result = new List<MaterialDtos>();
            var itemStyles = styles.Split(',');

            var productionBOMs = context.ItemStyle
                .Include(x => x.ProductionBOMs)
                .ThenInclude(y => y.ItemStyle)
                .Where(x => itemStyles.Contains(x.LSStyle));

            result = productionBOMs.SelectMany(x => x.ProductionBOMs)
                .Select(x => new MaterialDtos()
                {
                    ItemCode = x.ItemCode,
                    ItemID = x.ItemID,
                    ItemName = x.ItemName,
                    ItemColorCode = x.ItemColorCode,
                    ItemColorName = x.ItemColorName,
                    Specify = x.Specify,
                    Position = x.Position,
                    GarmentSize = x.GarmentSize,
                    MaterialType = x.MaterialTypeCode,
                    UnitID = x.PriceUnitID,
                    GarmentColorCode = x.ItemStyle.ColorCode,
                    GarmentColorName = x.ItemStyle.ColorName,
                    RequireQuantity = x.RequiredQuantity ?? 0,
                    ReservedQuantity = x.ReservedQuantity ?? 0,
                    RemainQuantity = x.RemainQuantity ?? 0,
                    LSStyle = x.ItemStyle.LSStyle,
                    CustomerStyle = x.ItemStyle.CustomerStyle
                     
                }).ToList();

            return result;
        }
        public IEnumerable<CustomerItemStyleDtos> GetCustomerStyles(string customerId)
        {                       
            var result = new List<CustomerItemStyleDtos>();
            result = context.ViewCustomerStyle
                .Select(x=>new CustomerItemStyleDtos
                {
                    CustomerStyle = x.CustomerStyle,
                    CustomerId = x.CustomerId,
                    CustomerName = x.CustomerName,
                    ItemStyleDescription = x.ItemStyleDescription
                })
                .Where(x=>x.CustomerId == customerId).ToList();
            return result;
        }
        public IEnumerable<LSStyleDtos> GetLSStyles(string customerStyle)
        {
            var result = new List<LSStyleDtos>();
            result = context.ItemStyle.Include(x=>x.SalesOrder)
                .Select(x => new LSStyleDtos
                {
                    SalesOrderID = x.SalesOrderID,
                    CustomerID = x.SalesOrder.CustomerID,
                    CustomerName = x.SalesOrder.CustomerName,
                    CustomerStyle = x.CustomerStyle,
                    LSStyle = x.LSStyle  
                })
                .Where(x=>x.CustomerStyle == customerStyle).ToList();
            return result;
        }
        public IEnumerable<LSStyleDtos> GetAllLSStyles()
        {
            var result = new List<LSStyleDtos>();
            result = context.ItemStyle.Include(x => x.SalesOrder)
                .Select(x => new LSStyleDtos
                {
                    SalesOrderID = x.SalesOrderID,
                    CustomerID = x.SalesOrder.CustomerID,
                    CustomerName = x.SalesOrder.CustomerName,
                    CustomerStyle = x.CustomerStyle,
                    LSStyle = x.LSStyle
                })
                .ToList();
            return result;
        }
        public IEnumerable<PONumberDto> GetAllPONumbers(string customerId)
        {
            var result = new List<PONumberDto>();
            result = context.ViewPONumber
                .Where(x => x.CustomerID == customerId)
                .Select(x => new PONumberDto
                {
                    PONumber = x.PurchaseOrderNumber,
                    LSStyle = x.LSStyle
                }).ToList();
            return result;
        }
    }
}
