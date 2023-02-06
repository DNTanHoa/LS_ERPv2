using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class JobHeadSearchParam
    {
        [Browsable(false)]
        [DevExpress.ExpressApp.Data.Key]
        public int ID { get; set; }
        public string Style { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public IList<JobHead> JobHeads { get; set; }
    }
}
