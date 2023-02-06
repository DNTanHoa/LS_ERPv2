using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ItemPrice : Audit
    {
        public long ID { get; set; }
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }
        public string Position { get; set; }
        public string Season { get; set; }
        public string LabelCode { get; set; }
        public string PriceUnitID { get; set; }
        public string CurrencyID { get; set; }
        public string CustomerID { get; set; }
        public string ShippingTermCode { get; set; }
        public decimal? Price { get; set; }
        public string VendorID { get; set; }
        public DateTime? EffectDate { get; set; }
        public string MaterialTypeCode { get; set; }

        public virtual Vendor Vendor { get; set; }
        public virtual Unit PriceUnit { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual ShippingTerm ShippingTerm { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
