using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IPackingListRepository
    {
        PackingList Add(PackingList packingList);
        void Update(PackingList packingList);
        void Delete(PackingList packingList);
        IQueryable<PackingList> GetPackingList();
        IQueryable<PackingList> GetPackingList(string Code, string CustomerID);
        IQueryable<PackingList> GetPackingList(long? InvoiceID);
    }
}
