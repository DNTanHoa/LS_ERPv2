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
    public class ItemModelRepository
        : IItemModelRepository
    {
        private readonly AppDbContext context;

        public ItemModelRepository(AppDbContext context)
        {
            this.context = context;
        }

        public ItemModel Add(ItemModel itemModel)
        {
            return context.ItemModel.Add(itemModel).Entity;
        }

        public void Delete(ItemModel itemModel)
        {
            context.ItemModel.Remove(itemModel);
        }

        public ItemModel GetItemModel(long ID)
        {
            return context.ItemModel
                .FirstOrDefault(x => x.ID == ID);
        }

        public IQueryable<ItemModel> GetItemModels()
        {
            return context.ItemModel;
        }

        public IQueryable<ItemModel> GetItemModels(string CustomerID)
        {
            return context.ItemModel.Where(x => x.CustomerID == CustomerID);
        }

        public bool IsExist(long ID, out ItemModel itemModel)
        {
            itemModel = null;
            itemModel = context.ItemModel.FirstOrDefault(x => x.ID == ID);
            return itemModel != null ? true : false;
        }

        public bool IsExist(long ID)
        {
            var itemModel = context.ItemModel.FirstOrDefault(x => x.ID == ID);
            return itemModel != null ? true : false;
        }

        public bool IsExistFile(string fileName)
        {
            var itemModel = context.ItemModel.FirstOrDefault(x => x.FileName == fileName);
            return itemModel != null ? true : false;
        }

        public void Update(ItemModel itemModel)
        {
            context.Entry(itemModel).State = EntityState.Modified;
        }
    }
}
