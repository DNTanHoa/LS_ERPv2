using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class SeparationPackingList : Audit
    {
        public int ID { get; set; }
        public int PackingListID { get; set; }
        public string CustomerID { get; set; }
        public decimal? TotalQuantity { get; set; }
        public int? SeparateOrdinal { get; set; } 
        public long? InvoiceID { get; set; } 
        public bool? IsShipped { get; set; }    
        public virtual Customer Customer { get; set; }
        public virtual IList<SeparationPackingLine> SeparationPackingLines { get; set; }
    }
}
