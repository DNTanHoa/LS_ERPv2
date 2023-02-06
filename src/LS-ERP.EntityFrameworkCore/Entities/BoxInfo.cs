using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class BoxInfo : Audit
    {
        public long ID { get; set; }
        public string TagID { get; set; }
        public string GarmentColorCode { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string GarmentSize { get; set; }
        public string CustomerStyle { get; set; }
        public decimal? QuantityPerBox { get; set; }
        public string CustomerID { get; set; }  
        public virtual Customer Customer { get; set; }  
    }
}
