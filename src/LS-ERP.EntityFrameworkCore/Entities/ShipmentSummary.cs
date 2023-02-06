using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ShipmentSummary : Audit
    {
        public long ID { get; set; }
        public long? ShipmentID { get; set; }
        public string VendorID { get; set; }
        public string CustomerPurchaseOrderNumber { get; set; }
        public DateTime? TrxDate { get; set; }
        public DateTime? MRQ { get; set; }
        public string DeliveryNo { get; set; }
        public string Description { get; set; }
        public string MaterialCode { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string UnitID { get; set; }
        public decimal? Quantity { get; set; }
        public bool? Received { get; set; }

        public virtual Shipment Shipment { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
