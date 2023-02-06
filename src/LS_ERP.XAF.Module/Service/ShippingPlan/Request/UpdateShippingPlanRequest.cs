using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class UpdateShippingPlanRequest
    {
        public int Id { get; set; }
        public string CustomerID { get; set; }
        public string CompanyID { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public List<ShippingPlanDetail> Details
            = new List<ShippingPlanDetail>();
    }
}
