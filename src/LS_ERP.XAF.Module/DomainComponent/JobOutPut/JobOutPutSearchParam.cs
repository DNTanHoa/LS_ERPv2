using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class JobOutPutSearchParam
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public Operation Operation { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
