using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class MaterialTransaction : Audit
    {
        public long ID { get; set; }

        /// <summary>
        /// Item master information
        /// </summary>
        public string ItemMasterID { get; set; }
        /// <summary>
        /// Item information
        /// </summary>
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Specify { get; set; }
        public string Season { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string UnitID { get; set; }
        public string OtherName { get; set; }
        public string DefaultThreadName { get; set; }

        /// <summary>
        /// Style information
        /// </summary>
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }

        /// <summary>
        /// Transaction information
        /// </summary>
        public decimal? RequestQuantity { get; set; } // for Fabric 
        public decimal? Quantity { get; set; }
        public decimal? CartonQuantity { get; set; }
        public decimal? Roll { get; set; }
        public DateTime? TransactionDate { get; set; }
        public bool? IsReversed { get; set; }
        public bool? IsProcessed { get; set; }
        public string StorageCode { get; set; }
        public string ProductionMethodCode { get; set; }
        public long? StorageDetailID { get; set; }

        /// <summary>
        /// Transaction source
        /// </summary>
        public string ReceiptNumber { get; set; }
        public long? ReceiptGroupLineID { get; set; }
        public string IssuedNumber { get; set; }
        public long? IssuedLineID { get; set; }
        public int StorageImportID { get; set; }
        public int StorageImportDetailID { get; set; }
        public long? FabricRequestDetailID { get; set; }

        /// <summary>
        /// Using bulk storage import
        /// </summary>
        public string PurchaseOrderNumber { get; set; }
        public string FabricPurchaseOrderNumber { get; set; }
        public string StorageBinCode { get; set; }
        public string LotNumber { get; set; }
        public string DyeLotNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceNumberNoTotal { get; set; }
        public string DocumentNumber { get; set; }
        public string Remark { get; set; }
        public string Zone { get; set; }
        public string UserFollow { get; set; }
        public string Note { get; set; }
        public string FabricContent { get; set; }
        public string StorageStatusID { get; set; }
        public string Supplier { get; set; }
        public bool? Offset { get; set; }
        public bool? OldImport { get; set; }
        /// <summary>
        /// QA 
        /// </summary>
        public long? QualityAssuranceID { get; set; }

        public virtual Receipt Receipt { get; set; }
        public virtual ReceiptGroupLine ReceiptGroupLine { get; set; }
        public virtual Issued Issued { get; set; }
        public virtual IssuedLine IssuedLine { get; set; }
    }
}
