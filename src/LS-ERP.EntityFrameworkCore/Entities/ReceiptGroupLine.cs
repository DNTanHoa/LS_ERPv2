using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ReceiptGroupLine : Audit
    {
        public ReceiptGroupLine()
        {
            Transactions = new List<MaterialTransaction>();
        }

        public long ID { get; set; }
        public string ReceiptNumber { get; set; }

        /// <summary>
        /// Item information
        /// </summary>
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }

        /// <summary>
        /// Item master information
        /// </summary>
        public string ItemMasterID { get; set; }

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
        public string GarmentSize { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }

        /// <summary>
        /// Quantity information
        /// </summary>
        public decimal? ReceiptQuantity { get; set; }
        public decimal? PurchaseQuantity { get; set; }
        public decimal? VendorDeliveriedQuantity { get; set; }
        public decimal? CartonQuantity { get; set; }
        public decimal? Roll { get; set; }

        /// <summary>
        /// Document information
        /// </summary>
        public string StorageBinCode { get; set; }
        public string Remark { get; set; }
        public string LotNumber { get; set; }
        public string DyeLotNumber { get; set; }
        public long? PurchaseOrderGroupLineID { get; set; }
        public long? FabricPurchaseOrderGroupLineID { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string FabricPurchaseOrderNumber { get; set; }
        public bool? Offset { get; set; }

        /// <summary>
        /// update information
        /// </summary>
        public string OtherName { get; set; }
        public string DefaultThreadName { get; set; }

        public virtual PurchaseOrderGroupLine PurchaseOrderGroupLine { get; set; }
        public virtual FabricPurchaseOrder FabricPurchaseOrderGroupLine { get; set; }
        public virtual Receipt Receipt { get; set; }
        public virtual Unit Unit { get; set; }

        public virtual IList<ReceiptLine> ReceiptLines { get; set; }
        public virtual IList<MaterialTransaction> Transactions { get; set; }
    }
}
