using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ScanResultDetailSummary
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public Company Company { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Search { get; set; }
        public List<ScanResultDetail> Results { get; set; } = new List<ScanResultDetail>();
        public List<SummaryScanResultDetail> Summaries { get; set; } = new List<SummaryScanResultDetail>();
    }

    [DomainComponent]
    public class SummaryScanResultDetail
    {
        public string PurchaseOrderNumber { get; set; }
        public string LSStyle { get; set; }
        public DateTime ScanDate { get; set; }
        public decimal OrderQuantity { get; set; }
        public decimal EntryQuantity { get; set; }
        public int TotalBox { get; set; }
        public decimal Percent { get; set; }
    }
}
