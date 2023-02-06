using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ItemStyleBarCode : Audit
    {
        public long ID { get; set; }
        public string ItemStyleNumber { get; set; }
        public string BarCode { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public string UE { get; set; }
        public string PCB { get; set; }
        public string Packing { get; set; }
        public decimal Quantity { get; set; }
        public int? SizeSortIndex { get; set; }

        public virtual ItemStyle ItemStyle { get; set; }
    }
}
