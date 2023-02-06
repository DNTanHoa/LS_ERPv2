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
    public class ItemPriceRepository : IItemPriceRepository
    {
        private readonly AppDbContext context;

        public ItemPriceRepository(AppDbContext context)
        {
            this.context = context;
        }

        public ItemPrice Add(ItemPrice itemPrice)
        {
            return context.ItemPrice.Add(itemPrice).Entity;
        }

        public void Delete(ItemPrice itemPrice)
        {
            context.ItemPrice.Remove(itemPrice);
        }

        public IQueryable<ItemPrice> GetItemPrices()
        {
            return context.ItemPrice;
        }

        public ItemPrice GetItemPrice(long ID)
        {
            return context.ItemPrice.FirstOrDefault(x => x.ID == ID);
        }

        public bool IsExist(long ID)
        {
            var itemPrice = GetItemPrice(ID);
            return itemPrice != null ? true : false;
        }

        public bool IsExist(long ID, out ItemPrice itemPrice)
        {
            itemPrice = null;
            itemPrice = GetItemPrice(ID);
            return itemPrice != null ? true : false;
        }

        public void Update(ItemPrice itemPrice)
        {
            context.Entry(itemPrice).State = EntityState.Modified;
        }

        public IQueryable<ItemPrice> GetItemPrices(string VendorID, 
            string ShippingTermCode, string CustomerID)
        {
            return context.ItemPrice.Where(x => x.CustomerID == CustomerID &&
                                                x.VendorID == VendorID &&
                                                x.ShippingTermCode == ShippingTermCode);
        }
    }
}
