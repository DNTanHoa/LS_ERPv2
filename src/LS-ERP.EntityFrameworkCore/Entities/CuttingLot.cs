using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class CuttingLot : Audit
    {
        public int ID { get; set; }
        public string Lot { get; set; }
        public string LSStyle { get; set; }
        public string Size { get; set; }
        public string Set { get; set; }
        public decimal Quantity { get; set; }
        public int AllocDailyOutputID { get; set; }
        public int  CuttingOutputID { get; set; }

    }
}
