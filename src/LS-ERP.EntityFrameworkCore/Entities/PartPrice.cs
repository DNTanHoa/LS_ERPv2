using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PartPrice : Audit
    {
        public PartPrice()
        {
            EffectiveDate = DateTime.Now;
            ExpiryDate = DateTime.MaxValue;
        }
        public int ID { get; set; }
        public string StyleNO { get; set; } = string.Empty;
        public decimal SMV { get; set; } = 0;
        public string Item { get; set; } = string.Empty;
        public string CompanyID { get; set; } = string.Empty;
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Remark { get; set; }
        public string Operation { get; set; }
        public string Season { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentSize { get; set; }
        public string ProductionType { get; set; }
        public decimal? Price { get; set; }
        public string CustomerID { get; set; }
    }
}
