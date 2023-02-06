using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IItemRepository
    {
        Item Add(Item item);
        void Update(Item item);
        void Delete(Item item);
        IQueryable<Item> GetItems();
        IQueryable<Item> GetItems(string CustomerID);
        Item GetItem(string Code);
        bool IsExist(Item item);
    }
}
