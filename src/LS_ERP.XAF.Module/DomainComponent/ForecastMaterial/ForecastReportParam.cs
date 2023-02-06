using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ForecastReportParam
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public string Search { get; set; }
        public Vendor Vendor { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<ForecastReprotDetail> Details { get; set; }
    }

    [DomainComponent]
    public class ForecastReprotDetail
    {
        public string ItemMasterID { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }

        public string CustomerStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string Season { get; set; }
        public string UnitID { get; set; }

        public decimal RequiredQuantity { get; set; }
        public decimal PurchaseQuantity { get; set; }
        public decimal BalanceQuantity { get; set; }
        public decimal RemainQuantity { get; set; }
        public decimal ReceiptQuantity { get; set; }
    }
}
