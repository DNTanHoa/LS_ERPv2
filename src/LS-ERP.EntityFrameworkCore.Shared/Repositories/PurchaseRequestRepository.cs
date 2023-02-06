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
    public class PurchaseRequestRepository
        : IPurchaseRequestRepository
    {
        private readonly AppDbContext context;

        public PurchaseRequestRepository(AppDbContext context)
        {
            this.context = context;
        }

        public PurchaseRequest Add(PurchaseRequest PurchaseRequest)
        {
            return context.PurchaseRequest.Add(PurchaseRequest).Entity;
        }

        public void Delete(PurchaseRequest PurchaseRequest)
        {
            context.PurchaseRequest.Remove(PurchaseRequest);
        }

        public PurchaseRequest GetPurchaseRequest(string ID)
        {
            return context.PurchaseRequest
                .FirstOrDefault(x => x.ID == ID);
        }

        public IQueryable<PurchaseRequest> GetPurchaseRequests()
        {
            return context.PurchaseRequest;
        }

        public bool IsExist(string ID, out PurchaseRequest PurchaseRequest)
        {
            PurchaseRequest = null;
            PurchaseRequest = context.PurchaseRequest.FirstOrDefault(x => x.ID == ID);
            return PurchaseRequest != null ? true : false;
        }

        public bool IsExist(string ID)
        {
            var PurchaseRequest = context.PurchaseRequest.FirstOrDefault(x => x.ID == ID);
            return PurchaseRequest != null ? true : false;
        }

        public void Update(PurchaseRequest PurchaseRequest)
        {
            context.Entry(PurchaseRequest).State = EntityState.Modified; 
        }
    }
}
