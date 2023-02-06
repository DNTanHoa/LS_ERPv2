using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class SalesOrderStatus : Audit
    {
        public SalesOrderStatus()
        {
            Code = string.Empty;
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
