using DevExpress.ExpressApp.DC;
using System;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ReceiptReportDetail
    {
        public string PurchaseOrderNumber { get; set; }
        public string FabricPurchaseOrderNumber { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceNumberNoTotal { get; set; }
        public string CustomerStyle { get; set; }
        public string Season { get; set; }
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Specify { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string CustomerID { get; set; }
        public string VendorID { get; set; }
        public string StorageCode { get; set; }
        public decimal PurchaseQuantity { get; set; }
        public decimal ReceiptQuantity { get; set; }

        /// <summary>
        /// Người nhập
        /// </summary>
        public string ReceiptBy { get; set; } = string.Empty;
    }
}
