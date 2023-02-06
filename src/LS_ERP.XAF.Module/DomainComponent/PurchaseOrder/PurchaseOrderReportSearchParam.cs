﻿using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class PurchaseOrderReportSearchParam
    {
        [Browsable(false)]
        [DevExpress.ExpressApp.Data.Key]
        public int ID { get; set; }
        public Customer Customer { get; set; }
        public Vendor Vendor { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<PurchaseOrderReportDetailParam> ReportDetails { get; set; } = new List<PurchaseOrderReportDetailParam>();
        public List<PurchaseOrderReportDetailParam> ListCharts { get; set; } = new List<PurchaseOrderReportDetailParam>();
        public List<PurchaseOrderReportDetailParam> ListPivot { get; set; } = new List<PurchaseOrderReportDetailParam>();
    }
}