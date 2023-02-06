using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SeasonReportParam
    {
        [DevExpress.ExpressApp.Data.Key]
        [Browsable(false)]
        public int ID { get; set; }
        public Customer Customer { get; set; }
        public string Season { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public List<SeasonReportDetail> Details { get; set; }
            = new List<SeasonReportDetail>();
    }
}
