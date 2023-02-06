using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class DailyTargetDetailSummaryByDate
    {
        [DevExpress.ExpressApp.Data.Key]
        [Browsable(false)]
        public int ID { get; set; }
        public Customer Customer { get; set; }
        public Operation Operation { get; set; }
        public DateTime ProduceDate { get; set; }
        public List<DailyTargetDetailSummaryByDateParam> ListDailyTargetDetailSummaryByDate { get; set; } = new List<DailyTargetDetailSummaryByDateParam>();
        public List<DailyTargetDetailSummaryByDateParam> ListDetail { get; set; } = new List<DailyTargetDetailSummaryByDateParam>();
    }
}
