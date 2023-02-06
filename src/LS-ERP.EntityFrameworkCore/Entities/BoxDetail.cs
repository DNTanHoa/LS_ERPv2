using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class BoxDetail : Audit
    {
        public BoxDetail()
        {
            Oid = Guid.NewGuid().ToString();
        }
        public string Oid { get; set; }
        public string GTIN { get; set; }
        public string Style { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int? Qty { get; set; }   
        public string Description { get; set; }
        public string ColorName { get; set; }
        public bool? IsMerge { get; set; }
        public int? OptimisticLockField { get; set; }
        public int? GCRecord { get; set; }
        public virtual BoxGroup BoxGroup { get; set; }
    }
}
