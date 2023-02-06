using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ShippingPlan : Audit
    {
        public int ID { get; set; }
        public string CustomerID { get; set; }
        public string CompanyID { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Company Company { get; set; }

        public virtual List<ShippingPlanDetail> Details { get; set; }
        = new List<ShippingPlanDetail>();
    }
}
