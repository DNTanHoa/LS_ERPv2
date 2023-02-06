using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore
{
    public class OrderDetailSync : Audit
    {
        public int ID { get; set; }
        public int ItemStyleSyncMasterID { get; set; }
        public string GarmentSize { get; set; }
        public decimal Quantity { get; set; }
        public virtual ItemStyleSyncMaster ItemStyleSyncMaster { get; set; }
    }
}
