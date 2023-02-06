using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ItemStyleSyncAction : Audit
    {
        /// <summary>
        /// Reference
        /// </summary>
        public int ID { get; set; }
        public int ItemStyleSyncMasterID { get; set; }
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }

        public virtual ItemStyleSyncMaster ItemStyleSyncMaster { get; set; }
    }
}
