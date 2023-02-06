using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PackingOverQuantity : Audit
    {
        public int ID { get; set; }
        public string PackingListCode { get; set; }
        public string ItemStyleNumber { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string Size { get; set; }
        public decimal? Quantity { get; set; }
        public int? SizeSortIndex { get; set; }
        public virtual PackingList PackingList { get; set; }
        public virtual ItemStyle ItemStyle { get; set; }
    }
}
