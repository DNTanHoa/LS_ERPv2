using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IInvoiceRepository
    {
        Invoice Add(Invoice invoice);
        void Update(Invoice invoice);
        void Delete(Invoice invoice);
        IQueryable<Invoice> GetInvoices();
        IQueryable<Invoice> GetInvoices(string CustomerID);
        Invoice GetInvoice(long? ID);
    }
}
