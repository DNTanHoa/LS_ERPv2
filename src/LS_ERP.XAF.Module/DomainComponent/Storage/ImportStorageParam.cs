using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ImportStorageParam
    {
        public string FilePath { get; set; }
        public Storage Storage { get; set; }
        public Customer Customer { get; set; }
        public bool Output { get; set; }
        public PriceTerm ProductionMethod { get; set; }
        public List<ImportStorageData> Data { get; set; }
            = new List<ImportStorageData>();
    }

    [DomainComponent]
    public class ImportStorageData
    {
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }
        public string Position { get; set; }

        /// <summary>
        /// Garment information
        /// </summary>
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string Season { get; set; }

        /// <summary>
        /// Unit
        /// </summary>
        public string UnitID { get; set; }
        public decimal Quantity { get; set; }
        public decimal Roll { get; set; }

        public string PurchaseOrderNumber { get; set; }
        public string FabricPurchaseOrderNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceNumberNoTotal { get; set; }
        public string StorageBinCode { get; set; }
        public string LotNumber { get; set; }
        public string DyeLotNumber { get; set; }
        public bool? Offset { get; set; }

        public string Remark { get; set; }
        public string Note { get; set; }
        public string Zone { get; set; }
        public string FabricContent { get; set; }
        public string DocumentNumber { get; set; }
        public string UserFollow { get; set; }
        public string Supplier { get; set; }
        public string StorageStatusID { get; set; }
        public long? StorageDetailID { get; set; }

        /// <summary>
        /// Output
        /// </summary>
        public decimal RollNo { get; set; }
        public string OutputOrder { get; set; }
        public string ProductionMethodCode { get; set; }

        public DateTime? TransactionDate { get; set; }
    }
}
