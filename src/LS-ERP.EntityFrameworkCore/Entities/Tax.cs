using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Tax : Audit
    {
        public Tax()
        {
            Code = string.Empty;
        }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
    }
}
