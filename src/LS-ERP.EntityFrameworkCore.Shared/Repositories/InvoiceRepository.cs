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
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext context;

        public InvoiceRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Invoice Add(Invoice invoice)
        {
            return context.Invoice.Add(invoice).Entity;
        }

        public void Delete(Invoice invoice)
        {
            context.Invoice.Remove(invoice);
        }

        public Invoice GetInvoice(long? ID)
        {
            return context.Invoice
                          .Include(x => x.InvoiceDetails)
                          .FirstOrDefault(x => x.ID == ID);
        }

        public IQueryable<Invoice> GetInvoices()
        {
            return context.Invoice;
        }

        public IQueryable<Invoice> GetInvoices(string CustomerID)
        {
            return context.Invoice.Where(x => x.CustomerID == CustomerID);
        }

        public void Update(Invoice invoice)
        {
            context.Entry(invoice).State = EntityState.Modified;
        }
    }
}
