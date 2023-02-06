using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Response
{
    public class PurchaseReceivedResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<PurchaseReceivedDetailsReport> Data { get; set; }
    }

    public class PurchaseReceivedDetailsReport
    {
        public string CustomerPurchaseOrderNumber { get; set; }
        public string ContractNo { get; set; }
        public string ItemID { get; set; }
        public string ItemColorName { get; set; }
        public string MaterialTypeClass { get; set; }
        public string CustomerStyle { get; set; }
        public string ItemNo { get; set; }
        public decimal? Quantity { get; set; }
        public DateTime? TrxDate { get; set; }
        public string DeliveryNo { get; set; }
        public long ShipmentID { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerID { get; set; }
        public string Number { get; set; }
        public DateTime? ArrivedDate { get; set; }
        public string StorageCode { get; set; }
    }
}
