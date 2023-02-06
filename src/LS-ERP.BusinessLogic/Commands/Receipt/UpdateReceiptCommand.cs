using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class UpdateReceiptCommand : CommonAuditCommand,
        IRequest<CommonCommandResult<Receipt>>
    {
        public string Number { get; set; }
        public string InvoiceNumber { get; set; }
        public string DocumentReferenceNumber { get; set; }
        public string ReceiptTypeId { get; set; }
        public string StorageCode { get; set; }
        public DateTime? ArrivedDate { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public DateTime? EntriedDate { get; set; }
        public string ReceiptBy { get; set; }
        public string EntriedBy { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string PurchaseOrderID { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string CustomerID { get; set; }

        public virtual IList<ReceiptGroupLine> ReceiptGroupLines { get; set; }
    }
}
