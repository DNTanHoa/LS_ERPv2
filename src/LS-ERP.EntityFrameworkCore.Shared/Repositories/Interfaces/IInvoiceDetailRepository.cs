using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IInvoiceDetailRepository
    {
        InvoiceDetail Add(InvoiceDetail invoiceDetail);
        void Update(InvoiceDetail invoiceDetail);
        void Delete(InvoiceDetail invoiceDetail);
        IQueryable<InvoiceDetail> GetInvoiceDetails();
        IQueryable<InvoiceDetail> GetInvoiceDetails(long? InvoiceID);
        InvoiceDetail GetInvoiceDetail(long ID);
    }
}
