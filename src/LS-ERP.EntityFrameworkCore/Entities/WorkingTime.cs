using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class WorkingTime : Audit
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int SortIndex { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }

    }
}
