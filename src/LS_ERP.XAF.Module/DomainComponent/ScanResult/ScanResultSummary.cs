using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ScanResultSummary
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public Company Company { get; set; }
        public DateTime Date { get; set; }
        public string Search { get; set; }
        public List<ScanResultDetail> Results { get; set; } = new List<ScanResultDetail>();
        public List<ScanResultSummaryDetail> Summaries { get; set; } = new List<ScanResultSummaryDetail>();
    }

    [DomainComponent]
    public class ScanResultSummaryDetail
    {
        public string PurchaseOrderNumber { get; set; }
        public string LSStyle { get; set; }
        public decimal OrderQuantity { get; set; }
        public decimal EntryQuantity { get; set; }
        public decimal TotalBox { get; set; }
        public decimal Percent { get; set; }
    }
}
