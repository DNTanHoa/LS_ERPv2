using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories
{
    public class ItemStyleRepository : IItemStyleRepository
    {
        private readonly AppDbContext context;

        public ItemStyleRepository(AppDbContext context)
        {
            this.context = context;
        }

        public ItemStyle Add(ItemStyle itemStyle)
        {
            return context.ItemStyle.Add(itemStyle).Entity;
        }

        public void Delete(ItemStyle itemStyle)
        {
            context.ItemStyle.Remove(itemStyle);
        }

        public ItemStyle GetItemStyle(string number)
        {
            return context.ItemStyle.FirstOrDefault(x => x.Number == number);
        }

        public IQueryable<ItemStyle> GetItemStyles()
        {
            return context.ItemStyle;
        }

        public IQueryable<ItemStyle> GetItemStyles(List<string> numbers)
        {
            return context.ItemStyle
                          .Include(x => x.ProductionBOMs)
                          .Include(x => x.OrderDetails)
                          .ThenInclude(x => x.ReservationEntries)
                          .ThenInclude(x => x.JobHead)
                          .Where(x => numbers.Contains(x.Number));
        }

        public IQueryable<ItemStyle> GetItemStyles(string CustomerID)
        {
            return context.ItemStyle
                .Include(x => x.SalesOrder)
                .Where(x => x.SalesOrder.CustomerID == CustomerID);
        }

        public IQueryable<ItemStyle> GetItemStylesFollowSalesOrderID(string orderID)
        {
            return context.ItemStyle
                          .Include(x => x.ProductionBOMs)
                          .Include(x => x.OrderDetails)
                          .ThenInclude(x => x.ReservationEntries)
                          .ThenInclude(x => x.JobHead)
                          .Where(x => orderID.Contains(x.SalesOrderID));
        }

        public IQueryable<ItemStyle> GetItemStylesFollowSalesOrderID(List<string> orderID)
        {
            return context.ItemStyle
                          .Include(x => x.SalesOrder)
                          .Include(x => x.OrderDetails)
                          .ThenInclude(x => x.ReservationEntries)
                          .ThenInclude(x => x.JobHead)
                          .Where(x => orderID.Contains(x.SalesOrderID));
        }

        public IQueryable<ItemStyle> GetItemStylesForCompareData(List<string> orderID)
        {
            return context.ItemStyle
                          .Include(x => x.SalesOrder)
                          .Include(x => x.OrderDetails)
                          .ThenInclude(x => x.ReservationEntries)
                          .ThenInclude(x => x.JobHead)
                          .Where(x => orderID.Contains(x.SalesOrderID)
                          && (x.IsNotCompare == null || (x.IsNotCompare != null && x.IsNotCompare == true))
                          && x.ItemStyleStatusCode != "3");
        }

        public IQueryable<ItemStyle> GetOnlyItemStylesFollowContractNo(List<string> contractNos)
        {
            return context.ItemStyle
                          .Where(x => contractNos.Contains(x.ContractNo));
        }

        public IQueryable<ItemStyle> GetOnlyItemStylesFollowSalesOrderID(List<string> orderID)
        {
            return context.ItemStyle
                          .Where(x => orderID.Contains(x.SalesOrderID) && x.ItemStyleStatusCode != "3");
        }

        public bool IsExist(string number)
        {
            var itemStyle = GetItemStyle(number);
            return itemStyle != null ? true : false;
        }

        public bool IsExist(string number, out ItemStyle itemStyle)
        {
            itemStyle = null;
            itemStyle = GetItemStyle(number);
            return itemStyle != null ? true : false;
        }

        public void Update(ItemStyle itemStyle)
        {
            context.Entry(itemStyle).State = EntityState.Modified;
        }

        /// Using import packing list HA
        public IQueryable<ItemStyle> GetItemStylesByPONumber(string PONumber)
        {
            return context.ItemStyle.Include(x => x.OrderDetails)
                          .Where(x => x.PurchaseOrderNumber.Contains(PONumber));
        }

        public IQueryable<ItemStyle> GetItemStylesForContractInfo(List<string> contractNos)
        {
            return context.ItemStyle
                           .Include(x => x.OrderDetails)
                           .Where(x => contractNos.Contains(x.ContractNo));
        }

        public IQueryable<ItemStyle> GetItemStylesFollowLSStyle(List<string> LSStyle)
        {
            return context.ItemStyle
                           .Include(x => x.OrderDetails)
                           .Where(x => LSStyle.Contains(x.LSStyle));
        }

        public IQueryable<ItemStyle> GetItemStylesFollowLSStyleBarcodes(List<string> LSStyle, string customerID)
        {
            return context.ItemStyle
                           .Include(x => x.Barcodes)
                           .Where(x => LSStyle.Contains(x.LSStyle) && x.SalesOrder.ID == customerID);
        }
    }
}
