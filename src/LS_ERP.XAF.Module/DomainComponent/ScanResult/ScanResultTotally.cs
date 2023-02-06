using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ScanResultTotally
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public Company Company { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Search { get; set; }
        public List<ScanResultTotal> Summaries { get; set; } = new List<ScanResultTotal>();
    }

    [DomainComponent]
    public class ScanResultTotal
    {
        public string PurchaseOrderNumber { get; set; }
        public string LSStyle { get; set; }
        public decimal? OrderQuantity { get; set; }
        public int? EntryQuantity { get; set; }
        public string Status { get; set; }
    }
}
