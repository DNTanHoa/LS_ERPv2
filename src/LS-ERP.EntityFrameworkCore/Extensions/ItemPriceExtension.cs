using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Extensions
{
    public static class ItemPriceExtension
    {
        public static string ToSearchKey(this ItemPrice item)
        {
            return item.ItemName?.Replace(" ", "").ToUpper()
                   + (item.MaterialTypeCode ?? string.Empty);
        }
    }
}
