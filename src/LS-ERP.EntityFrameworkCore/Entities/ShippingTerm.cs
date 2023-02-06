using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ShippingTerm : Audit
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; }
    }
}
