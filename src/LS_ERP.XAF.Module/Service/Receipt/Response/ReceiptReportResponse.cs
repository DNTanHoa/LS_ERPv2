using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Response
{
    public class ReceiptReportResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<ReceiptReportResponseData> Data { get; set; }
    }

    public class ReceiptReportResponseData
    {
        public string PurchaseOrderNumber { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string InvoiceNumber { get; set; }
        public string Season { get; set; }
        public string CustomerStyle { get; set; }
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
        /// Người nhận
        /// </summary>
        public string ReceiptBy { get; set; }
    }
}
