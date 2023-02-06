using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ImportShippingPlanParam
    {
        public Customer Customer { get; set; }
        public Company Company { get; set; }
        public string FilePath { get; set; }
        public List<ShippingPlanDetail> Details { get; set; }
            = new List<ShippingPlanDetail>();
    }
}
