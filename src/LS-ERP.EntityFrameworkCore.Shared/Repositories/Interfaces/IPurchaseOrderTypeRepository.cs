using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IPurchaseOrderTypeRepository
    {
        PurchaseOrderType Add(PurchaseOrderType PurchaseOrderType);
        void Update(PurchaseOrderType PurchaseOrderType);
        void Delete(PurchaseOrderType PurchaseOrderType);
        IQueryable<PurchaseOrderType> GetPurchaseOrderTypes();
        PurchaseOrderType GetPurchaseOrderType(string Code);
        bool IsExist(string Code, out PurchaseOrderType PurchaseOrderType);
        bool IsExist(string Code);
    }
}
