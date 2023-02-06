using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class DailyTargetSearchParam
    {
        [DevExpress.ExpressApp.Data.Key]
        [Browsable(false)]
        public int ID { get; set; }
        public string CompanyID { get; set; }
        public DateTime ProduceDate { get; set; }
        public List<DailyTargetReportParam> DailyTargets { get; set; }
    }

}
