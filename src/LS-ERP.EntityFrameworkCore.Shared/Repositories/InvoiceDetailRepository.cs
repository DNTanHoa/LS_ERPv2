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
    public class InvoiceDetailRepository : IInvoiceDetailRepository
    {
        private readonly AppDbContext context;

        public InvoiceDetailRepository(AppDbContext context)
        {
            this.context = context;
        }

        public InvoiceDetail Add(InvoiceDetail invoiceDetail)
        {
            return context.InvoiceDetail.Add(invoiceDetail).Entity;
        }

        public void Delete(InvoiceDetail invoiceDetail)
        {
            context.InvoiceDetail.Remove(invoiceDetail);
        }

        public InvoiceDetail GetInvoiceDetail(long ID)
        {
            return context.InvoiceDetail.FirstOrDefault(x => x.ID == ID);
        }

        public IQueryable<InvoiceDetail> GetInvoiceDetails()
        {
            return context.InvoiceDetail;
        }

        public IQueryable<InvoiceDetail> GetInvoiceDetails(long? InvoiceID)
        {
            return context.InvoiceDetail.Where(x => x.InvoiceID == InvoiceID);
        }

        public void Update(InvoiceDetail invoiceDetail)
        {
            context.Entry(invoiceDetail).State = EntityState.Modified;
        }
    }
}
