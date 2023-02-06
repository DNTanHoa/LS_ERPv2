using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IPurchaseRequestRepository
    {
        PurchaseRequest Add(PurchaseRequest PurchaseRequest);
        void Update(PurchaseRequest PurchaseRequest);
        void Delete(PurchaseRequest PurchaseRequest);
        IQueryable<PurchaseRequest> GetPurchaseRequests();
        PurchaseRequest GetPurchaseRequest(string Number);
        bool IsExist(string ID, out PurchaseRequest PurchaseRequest);
        bool IsExist(string ID);
    }
}
