using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Receipt : Audit
    {
        public Receipt()
        {
            Number = string.Empty;
        }
        public string Number { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceNumberNoTotal { get; set; }
        public string ReceiptTypeId { get; set; }
        public string DocumentReferenceNumber { get; set; }
        public string StorageCode { get; set; }
        public string CustomerStyle { get; set; }
        public bool? Offset { get; set; }

        /// <summary>
        /// Document date information
        /// </summary>
        public DateTime? ArrivedDate { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public DateTime? AcountingDate { get; set; }
        public DateTime? EntriedDate { get; set; }

        /// <summary>
        /// Document person information
        /// </summary>
        public string ReceiptBy { get; set; }
        public string EntriedBy { get; set; }
        public string AccountingBy { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }

        /// <summary>
        /// Link to purchase
        /// </summary>
        public string PurchaseOrderID { get; set; }
        public long? FabricPurchaseOrderID { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string FabricPurchaseOrderNumber { get; set; }

        /// <summary>
        /// Finance information
        /// </summary>
        public string AccountDebitNumber { get; set; }
        public string AccountCreditNumber { get; set; }

        /// <summary>
        /// Addtional information
        /// </summary>
        public string CustomerID { get; set; }
        public string ProductionMethodCode { get; set; }

        public virtual Vendor Vendor { get; set; }
        public virtual PriceTerm ProductionMethod { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual FabricPurchaseOrder FabricPurchaseOrder { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Storage Storage { get; set; }

        public virtual IList<ReceiptGroupLine> ReceiptGroupLines { get; set; }
    }
}
