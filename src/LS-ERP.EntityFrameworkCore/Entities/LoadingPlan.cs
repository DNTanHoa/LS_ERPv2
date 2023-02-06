using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class LoadingPlan : Audit
    {
        public long ID { get; set; }    
        public string ContainerNumber { get; set; }
        public string ASNumber{ get; set; }
        public string TiersName { get; set; }
        public string Shu { get; set; } 
        public string OrderNumber { get; set; }
        public string ItemCode { get; set; }
        public decimal? PCB { get; set; }
        public string Port { get; set; }
        public int? Rank { get; set; }
        public string ORINumber { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? Quantity { get; set; }
        public string ModelCode { get; set; }
        public string Destination { get; set; }
        public decimal? Volumn { get; set; }
        public string Description { get; set; }
        public string CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
