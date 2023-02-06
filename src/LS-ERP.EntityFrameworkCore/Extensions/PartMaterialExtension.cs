using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.Ultilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Extensions
{
    public static class PartMaterialExtension
    {
        public static string ToSearchKey(this PartMaterial partMaterial, 
            string customerID, string season)
        {
            return partMaterial.ItemName?.Replace(" ", "").ToUpper()
                            + (partMaterial.MaterialTypeCode ?? string.Empty);
        }
    }
}
