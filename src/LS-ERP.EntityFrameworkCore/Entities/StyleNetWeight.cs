using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class StyleNetWeight : Audit
    {
        public int ID { get; set; }
        public string CustomerStyle { get; set; }
        public string Size { get; set; }
        public decimal? NetWeight { get; set; }
        public string CustomerID { get; set; }

        /// PKL for DE
        public string BoxDimensionCode { get; set; }
        public decimal? BoxWeight { get; set; }
        public string GarmentColorCode { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
