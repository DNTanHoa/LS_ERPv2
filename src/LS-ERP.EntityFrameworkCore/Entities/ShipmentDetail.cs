using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ShipmentDetail : Audit
    {
        public long ID { get; set; }
        public long? ShipmentID { get; set; }
        public string ContractNo { get; set; }
        public string CustomerPurchaseOrderNumber { get; set; }
        public DateTime? TrxDate { get; set; }
        public DateTime? MRQ { get; set; }
        public string DeliveryNo { get; set; }
        public string MaterialClass { get; set; }
        public string MaterialCode { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public decimal? AllocQuantity { get; set; }
        public decimal? AllocReceivedQuantity { get; set; }
        public bool? MatchedPO { get; set; }
        public virtual Shipment Shipment { get; set; }

        public string KeyShipmentDetail()
        {
            var key = string.Empty;
            key = this.CustomerPurchaseOrderNumber + this.ContractNo + this.MaterialCode + this.Color;
            return key;
        }
    }
}
