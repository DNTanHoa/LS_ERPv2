using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class BoxModel : Audit
    {
        public BoxModel()
        {
            Oid = Guid.NewGuid().ToString();
        }
        public string Oid { get; set; }
        public string Barcode { get; set; }
        public int? OptimisticLockField { get; set; }
        public int? GCRecord { get; set; }
        public virtual BoxGroup BoxGroup { get; set; }
    }
}
