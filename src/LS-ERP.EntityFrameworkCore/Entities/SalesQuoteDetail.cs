using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class SalesQuoteDetail
    {
        public long ID { get; set; }
        public string ExternalCode { get; set; }
        public long? SaleQuoteID { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string Position { get; set; }

        public decimal? Consumption { get; set; }
        public decimal? QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? WastagePercent { get; set; }
        
        public string UnitID { get; set; }
        public string PriceUnitID { get; set; }
        public string VendorID { get; set; }
        public decimal? Amount { get; set; }
        public string Note { get; set; }
        public string MaterialTypeCode { get; set; }
        public int? Type { get; set; }

        public virtual SalesQuote SalesQuote { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual MaterialType MaterialType { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual Unit PriceUnit { get; set; }
    }
}
