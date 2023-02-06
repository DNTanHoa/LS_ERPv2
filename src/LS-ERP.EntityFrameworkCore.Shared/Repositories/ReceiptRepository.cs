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
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly AppDbContext context;

        public ReceiptRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Receipt Add(Receipt receipt)
        {
            return context.Receipt.Add(receipt).Entity;
        }

        public void Delete(Receipt receipt)
        {
            context.Receipt.Remove(receipt);
        }

        public Receipt GetReceipt(string number)
        {
            return context.Receipt
                .Include(x => x.Customer)
                .Include(x => x.ReceiptGroupLines)
                .Include(x => x.PurchaseOrder)
                .Include(x => x.Vendor)
                .FirstOrDefault(x => x.Number == number);
        }

        public IEnumerable<Receipt> GetReceipts()
        {
            return context.Receipt;
        }

        public bool IsExist(string number, out Receipt receipt)
        {
            receipt = null;
            receipt = context.Receipt.FirstOrDefault(x => x.Number == number);
            return receipt != null ? true : false;
        }

        public bool IsExist(string number)
        {
            var receipt = context.Receipt.FirstOrDefault(x => x.Number == number);
            return receipt != null ? true : false;
        }

        public void Update(Receipt receipt)
        {
            context.Entry(receipt).State = EntityState.Modified;
        }
    }
}
