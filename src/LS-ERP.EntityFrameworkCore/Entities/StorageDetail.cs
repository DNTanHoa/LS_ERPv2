using System.Collections.Generic;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class StorageDetail
    {
        public StorageDetail()
        {
            this.ReservationEntries = new List<ReservationEntry>();
            this.MaterialTransactions = new List<MaterialTransaction>();
        }

        public long ID { get; set; }
        public string StorageCode { get; set; }

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
        public decimal? OnHandQuantity { get; set; }
        public decimal? ReseveredQuantity { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? NotReceivedQuantity { get; set; }
        public decimal? IssuedQuantity { get; set; }
        public decimal? CartonQuantity { get; set; }

        public decimal? CanUseQuantity { get; set; }
        public decimal? CanReUseQuantity { get; set; }
        public decimal? Roll { get; set; }
        /// <summary>
        /// Document information
        /// </summary>
        public string StorageBinCode { get; set; }
        public string LotNumber { get; set; }
        public string DyeLotNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceNumberNoTotal { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string FabricPurchaseOrderNumber { get; set; }
        public string DocumentNumber { get; set; }
        public string Remark { get; set; }
        public string Zone { get; set; }
        public string UserFollow { get; set; }
        public string FabricContent { get; set; }
        public string ProductionMethodCode { get; set; }
        public bool? Offset { get; set; }

        /// <summary>
        /// External information
        /// </summary>
        public string CustomerID { get; set; }

        public string Note { get; set; }
        public bool? OldImport { get; set; }

        /// <summary>
        /// For QA
        /// </summary>
        public string StorageStatusID { get; set; }

        /// <summary>
        /// Item master information
        /// </summary>
        public string ItemMasterID { get; set; }
        public string OtherName { get; set; }
        public string DefaultThreadName { get; set; }

        public virtual PriceTerm ProductionMethod { get; set; }
        public virtual Storage Storage { get; set; }
        public virtual StorageStatus StorageStatus { get; set; }

        public virtual IList<ReservationEntry> ReservationEntries { get; set; }
        public virtual IList<MaterialTransaction> MaterialTransactions { get; set; }
    }
}