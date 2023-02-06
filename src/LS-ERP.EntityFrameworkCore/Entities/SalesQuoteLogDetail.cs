using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class SalesQuoteLogDetail : Audit
    {
        public long ID { get; set; }
        public string SalesQuoteLogCode { get; set; }

        public virtual SalesQuoteLog SalesQuoteLog { get; set; }
    }
}
