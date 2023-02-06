using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Extensions
{
    public static class PackingLineExtension
    {
        public static void UpdateGrossWeight(this PackingLine line)
        {
            line.GrossWeight = line.NetWeight + line.BoxDimension?.Weight * line.TotalCarton
                + line.InnerBoxDimension?.Weight * line.TotalCarton * line.PackagesPerBox;
        }
    }
}
