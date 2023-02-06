using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class SalesQuoteStatus : Audit
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? CanEdit { get; set; }
    }
}
