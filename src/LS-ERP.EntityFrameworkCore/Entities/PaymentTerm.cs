using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PaymentTerm : Audit
    {
        public PaymentTerm()
        {
            Code = string.Empty;
        }

        public string Code { get; set; }
        public string Description { get; set; }
        public int DueDays { get; set; }
        
    }
}
