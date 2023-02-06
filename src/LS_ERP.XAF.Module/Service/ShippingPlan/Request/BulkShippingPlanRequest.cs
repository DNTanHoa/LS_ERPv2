using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class BulkShippingPlanRequest
    {
        public string UserName { get; set; }
        public string CustomerID { get; set; }
        public string CompanyID { get; set; }
        public List<ShippingPlanDetail> Data { get; set; } = new List<ShippingPlanDetail>();
    }
}
