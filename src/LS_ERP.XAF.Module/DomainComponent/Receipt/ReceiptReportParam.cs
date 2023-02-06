using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ReceiptReportParam
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public Storage Storage { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<ReceiptReportDetail> Details { get; set; } = new List<ReceiptReportDetail>();
    }
}
