using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Size : Audit
    {
        public long ID { get; set; }
        public string CustomerID { get; set; }
        public int? SequeneceNumber { get; set; }
        public string Code { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
