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
    public class PurchaseOrderTypeRepository : IPurchaseOrderTypeRepository
    {
        private readonly AppDbContext context;

        public PurchaseOrderTypeRepository(AppDbContext context)
        {
            this.context = context;
        }

        public PurchaseOrderType Add(PurchaseOrderType PurchaseOrderType)
        {
            return context.PurchaseOrderType.Add(PurchaseOrderType).Entity;
        }

        public void Delete(PurchaseOrderType PurchaseOrderType)
        {
            context.PurchaseOrderType.Remove(PurchaseOrderType);
        }

        public PurchaseOrderType GetPurchaseOrderType(string Code)
        {
            return context.PurchaseOrderType.FirstOrDefault(x => x.Code == Code);
        }

        public IQueryable<PurchaseOrderType> GetPurchaseOrderTypes()
        {
            return context.PurchaseOrderType;
        }

        public bool IsExist(string Code, out PurchaseOrderType PurchaseOrderType)
        {
            PurchaseOrderType = null;
            PurchaseOrderType = GetPurchaseOrderType(Code);
            return PurchaseOrderType != null ? true : false;
        }

        public bool IsExist(string Code)
        {
            var PurchaseOrderType = GetPurchaseOrderType(Code);
            return PurchaseOrderType != null ? true : false;
        }

        public void Update(PurchaseOrderType PurchaseOrderType)
        {
            context.Entry(PurchaseOrderType).State = EntityState.Modified;
        }
    }
}
