using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Extensions
{
    public static class MaterialTransactionExtension
    {
        public static void ReverseAll(this List<MaterialTransaction> materialTransactions)
        {
            materialTransactions.ForEach(x =>
            {
                x.IsReversed = true;
            });
        }
    }
}
