using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PurchaseRequestLog : Audit
    {
        public PurchaseRequestLog()
        {
            this.Code = string.Empty;
        }

        public string Code { get; set; }
        public string PurchaseRequestID { get; set; }

        public DateTime? ActivityDate { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public string ReferenceLogCode { get; set; }
        
        public virtual PurchaseRequestLog ReferenceLog { get; set; }
        public virtual PurchaseRequest PurchaseRequest { get; set; }
    }
}
