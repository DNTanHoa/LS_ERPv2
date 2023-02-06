using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class SalesQuoteLog : Audit
    {
        public long? SaleQuoteID { get; set; }
        public string Code { get; set; }
        public DateTime? ActivyDate { get; set; }
        public string Description { get; set; }
        public string SalesQuoteLogReferenceCode { get; set; }

        public virtual SalesQuoteLog SalesQuoteLogReference { get; set; }
        public virtual SalesQuote SalesQuote { get; set; }
    }
}
