using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Brand : Audit
    {
        public Brand()
        {
            Code = string.Empty;
        }

        public string Code { get; set; }
        public string CustomerID { get; set; }
        public string Name { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
