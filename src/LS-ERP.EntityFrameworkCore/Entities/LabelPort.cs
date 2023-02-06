using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class LabelPort : Audit
    {
        public long ID { get; set; }
        public string Division { get; set; }
        public string LabelCode { get; set; }
        public string LabelName { get; set; }
        public string ETAPort { get; set; }
        public string CustomerID { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
