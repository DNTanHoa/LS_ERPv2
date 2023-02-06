using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class CreateReceiptRequest
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceNumberNoTotal { get; set; }
        public string DocumentReferenceNumber { get; set; }
        public string StorageCode { get; set; }
        public string ReceiptTypeId { get; set; }
        public DateTime? ArrivedDate { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public DateTime? EntriedDate { get; set; }
        public string ReceiptBy { get; set; }
        public string EntriedBy { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string PurchaseOrderID { get; set; }
        public long? FabricPurchaseOrderID { get; set; }
        public string FabricPurchaseOrderNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string CustomerID { get; set; }
        public string CustomerStyle { get; set; }
        public string Username { get; set; }
        public string ProductionMethodCode { get; set; }
        public bool? Offset { get; set; }

        public IEnumerable<ReceiptGroupLineRequest> ReceiptGroupLines { get; set; }
    }

    public class ReceiptGroupLineRequest
    {
        public string ReceiptNumber { get; set; }
        public string ItemMasterID { get; set; }

        /// <summary>
        /// Item information
        /// </summary>
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
        public decimal? ReceiptQuantity { get; set; }
        public decimal? PurchaseQuantity { get; set; }
        public decimal? CartonQuantity { get; set; }
        public decimal? VendorDeliveriedQuantity { get; set; }
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
    }
}
