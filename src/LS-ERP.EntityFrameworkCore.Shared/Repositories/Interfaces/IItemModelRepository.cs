using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IItemModelRepository
    {
        ItemModel Add(ItemModel itemModel);
        void Update(ItemModel itemModel);
        void Delete(ItemModel itemModel);
        IQueryable<ItemModel> GetItemModels();
        IQueryable<ItemModel> GetItemModels(string CustomerID);
        ItemModel GetItemModel(long ID);
        bool IsExist(long ID, out ItemModel itemModel);
        bool IsExist(long ID);
        bool IsExistFile(string fileName);
    }
}
