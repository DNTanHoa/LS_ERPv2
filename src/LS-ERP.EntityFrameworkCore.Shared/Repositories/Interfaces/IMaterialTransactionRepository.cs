using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IMaterialTransactionRepository
    {
        IQueryable<MaterialTransaction> GetMaterialTransactions();
        IQueryable<MaterialTransaction> GetMaterialTransactions(string storageCode);
        IQueryable<MaterialTransaction> GetMaterialTransactions(string storageCode, 
            DateTime fromDate, DateTime toDate);

        IQueryable<MaterialTransaction> GetMaterialTransactionsOfReceipt(string ReceiptNumber);
        IQueryable<MaterialTransaction> GetMaterialTransactionsOfIssued(string IssuedNumber);
    }
}
