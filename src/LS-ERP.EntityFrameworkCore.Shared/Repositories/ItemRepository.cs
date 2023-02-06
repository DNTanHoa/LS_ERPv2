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
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext context;

        public ItemRepository(AppDbContext context)
        {
            this.context = context;
        }
        public Item Add(Item item)
        {
            return context.Item.Add(item).Entity;
        }

        public void Delete(Item item)
        {
            context.Item.Remove(item);
        }

        public Item GetItem(string Code)
        {
            return context.Item.FirstOrDefault(x => x.Code == Code);
        }

        public IQueryable<Item> GetItems()
        {
            return context.Item;
        }

        public IQueryable<Item> GetItems(string CustomerID)
        {
            return context.Item.Where(x => x.CustomerID == CustomerID);
        }

        public bool IsExist(Item item)
        {
            var itemExist = GetItem(item.Code);
            return itemExist != null ? true : false;
        }

        public void Update(Item item)
        {
            context.Entry(item).State = EntityState.Modified;
        }
    }
}
