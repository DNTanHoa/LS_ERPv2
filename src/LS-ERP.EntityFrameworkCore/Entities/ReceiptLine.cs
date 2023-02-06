using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ReceiptLine : Audit
    {
        public long ID { get; set; }
        public long ReceiptGroupLineID { get; set; }

        /// <summary>
        /// Item master information
        /// </summary>
        public string ItemMasterID { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Specify { get; set; }
        public string Season { get; set; }
        public string UnitID { get; set; }

        /// <summary>
        /// Garment information
        /// </summary>
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }

        /// <summary>
        /// Quantity information
        /// </summary>
        public decimal? Quantity { get; set; }
        public decimal? CartonQuantity { get; set; }
        public decimal? Roll { get; set; }

        /// <summary>
        /// Document information
        /// </summary>
        public string StorageBinCode { get; set; }
        public string LotNumber { get; set; }
        public string NyeLotNumber { get; set; }
        public long? PurchaseOrderLineID { get; set; }
        public long? FabricPurchaseOrderID { get; set; }

        public virtual ReceiptGroupLine ReceiptGroupLine { get; set; }
        public virtual PurchaseOrderLine PurchaseOrderLine { get; set; }
        public virtual FabricPurchaseOrder FabricPurchaseOrder { get; set; }
    }
}
