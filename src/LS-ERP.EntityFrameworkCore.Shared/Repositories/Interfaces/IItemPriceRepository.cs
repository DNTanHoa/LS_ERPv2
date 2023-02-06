using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IItemPriceRepository
    {
        ItemPrice Add(ItemPrice itemPrice);
        void Update(ItemPrice itemPrice);
        void Delete(ItemPrice itemPrice);
        IQueryable<ItemPrice> GetItemPrices();
        IQueryable<ItemPrice> GetItemPrices(string VendorID, string ShippingTermCode, string CustomerID);
        ItemPrice GetItemPrice(long ID);
        bool IsExist(long ID);
        bool IsExist(long ID, out ItemPrice itemPrice);
    }
}
