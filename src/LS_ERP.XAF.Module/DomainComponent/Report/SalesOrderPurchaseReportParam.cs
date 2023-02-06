using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SalesOrderPurchaseReportParam
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public Customer Customer { get; set; }
        public string SalesOrderNo { get; set; }
        public string Style { get; set; }
        public List<SalesOrderPurchaseReportDetail> OrderDetails { get; set; }
            = new List<SalesOrderPurchaseReportDetail>();
    }

    [DomainComponent]
    public class SalesOrderPurchaseReportDetail
    {
        public string SalesOrderID { get; set; }
        public string SalesOrderMS { get; set; }
        public DateTime? OrderDate { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public DateTime? PurchaseOrderDate { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PurchaseOrderID { get; set; }
        public string VenderID { get; set; }
        public string SaleOrderMS { get; set; }
        public string PurchaseOrderMS { get; set; }
    }

    [DomainComponent]
    public class SalesOrderPurchaseReportLine
    {
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Specify { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public decimal RequiredQuantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal PurchasedQuantity { get; set; }
        public decimal ReceiptQuantity { get; set; }
    }
}
