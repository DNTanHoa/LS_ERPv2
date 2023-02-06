using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PartMeta
    {
        public long ID { get; set; }
        public string PartNumber { get; set; }
        public string MetaKey { get; set; }
        public string MetaValue { get; set; }
        public string MetaDisplayName { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
