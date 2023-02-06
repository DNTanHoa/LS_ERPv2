using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories
{
    public class MaterialTransactionRepository : IMaterialTransactionRepository
    {
        private readonly AppDbContext context;

        public MaterialTransactionRepository(AppDbContext context)
        {
            this.context = context;
        }

        public IQueryable<MaterialTransaction> GetMaterialTransactions()
        {
            return context.MaterialTransaction;
        }

        public IQueryable<MaterialTransaction> GetMaterialTransactions(string storageCode)
        {
            return context.MaterialTransaction.Where(x => x.StorageCode == storageCode);
        }

        public IQueryable<MaterialTransaction> GetMaterialTransactions(string storageCode, 
            DateTime fromDate, DateTime toDate)
        {
            return context.MaterialTransaction.Where(x => x.StorageCode == storageCode &&
                                                          x.TransactionDate.Value.Date >= fromDate.Date &&
                                                          x.TransactionDate.Value.Date <= toDate.Date);
        }

        public IQueryable<MaterialTransaction> GetMaterialTransactionsOfIssued(string IssuedNumber)
        {
            return context.MaterialTransaction.Where(x => x.IssuedNumber == IssuedNumber);
        }

        public IQueryable<MaterialTransaction> GetMaterialTransactionsOfReceipt(string ReceiptNumber)
        {
            return context.MaterialTransaction.Where(x => x.ReceiptNumber == ReceiptNumber);
        }
    }
}
