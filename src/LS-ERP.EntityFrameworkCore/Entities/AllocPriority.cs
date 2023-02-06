using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class AllocPriority : Audit
    {
       
        public int ID { get; set; }
        public string CustomerID { get; set; }
        public string Operation { get; set; }
        public string PriorityName { get; set; }
        public int Index { get; set; }

    }
}
