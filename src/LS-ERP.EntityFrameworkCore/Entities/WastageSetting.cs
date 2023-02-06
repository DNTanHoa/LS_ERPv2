using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class WastageSetting
    {
        public long ID { get; set; }
        public string CustomerID { get; set; }
        public string MaterialTypeCode { get; set; }
        public string MaterialTypeClass { get; set; }

        public int? Rounding { get; set; }
        public decimal? WastageStandard { get; set; }
        public string RangeQuantity { get; set; }

        public decimal? MinQuantity { get; set; }
        public decimal? MaxQuantity { get; set; }

        public virtual MaterialType MaterialType { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
