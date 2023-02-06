using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Extensions
{
    public static class StorageDetailExtension
    {
        public static void CalculateQuantity(this StorageDetail storageDetail)
        {
            storageDetail.OnHandQuantity = storageDetail.MaterialTransactions.Where(x => !(x.IsReversed == true))
                .Sum(x => x.Quantity);

            storageDetail.Roll = storageDetail.MaterialTransactions.Where(x => !(x.IsReversed == true))
                .Sum(x => x.Roll);

            storageDetail.ReseveredQuantity = storageDetail.ReservationEntries
                .Sum(x => x.ReservedQuantity - x.IssuedQuantity);

            storageDetail.CanUseQuantity = storageDetail.OnHandQuantity - (storageDetail.ReseveredQuantity ?? 0);
        }
    }
}
