using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class InventoryPeriod : Audit
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsClosed { get; set; }
        public string StorageCode { get; set; } 
        public virtual IList<InventoryPeriodEntry> Entries { get; set; }
    }
}
