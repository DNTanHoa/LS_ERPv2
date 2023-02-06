using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class MaterialSyncDetail : Audit
    {
        public int ID { get; set; }
        public int MaterialSyncID { get; set; }
        public string ActionName { get; set; }
        public DateTime? ActionDate { get; set; }
        public decimal Quantity { get; set; }

        public virtual MaterialSync MaterialSync { get; set; }
    }
}
