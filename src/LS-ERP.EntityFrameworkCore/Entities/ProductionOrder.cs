using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ProductionOrder : Audit
    {
        public ProductionOrder()
        {
            ID = "ProOr" + DateTime.Now.Year + "-" + 
                Nanoid.Nanoid.Generate("ABCDEFGHIJKLMNOPQRST123456789", 15);
        }
        public string ID { get; set; }
        public string ProductionOrderTypeID { get; set; }
        public string CustomerID { get; set; }
        public string ReferenceID { get; set; }
        public DateTime? EstimateStartDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EstimateEndDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
