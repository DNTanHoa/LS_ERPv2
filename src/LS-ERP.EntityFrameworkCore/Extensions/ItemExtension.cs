using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Extensions
{
    public static class ItemExtension
    {
        public static string ToSearchKey(this Item item)
        {
            return item.Name?.Replace(" ", "").ToUpper()
                   + (item.MaterialTypeCode ?? string.Empty);
        }
    }
}
