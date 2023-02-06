using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class DailyTargetDetailSummary
    {
        [DevExpress.ExpressApp.Data.Key]
        [Browsable(false)]
        public int ID { get; set; }
        public Customer Customer { get; set; }
        public string Style { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<DailyTargetDetailReportParam> DailyTargetDetailReports { get; set; }
    }
}
