using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent.Report;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class InventoryReportParam
    {
        [DevExpress.ExpressApp.Data.Key]
        [Browsable(false)]
        public int ID { get; set; }
        public string Search { get; set; }
        public string PurchaseOrder { get; set; }
        public Storage Storage { get; set;  }
        public Customer Customer { get; set; }
        public List<InventoryReportDetail> Details { get; set; }
        = new List<InventoryReportDetail>();
    }
}
