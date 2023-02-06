using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ItemMeta : Audit
    {
        public long ID { get; set; }
        public string ItemID { get; set; }
        public string MetaKey { get; set; }
        public string MetaValue { get; set; }
        public string MetaDisplayname { get; set; }

        public virtual Item Item { get; set; }
    }
}
