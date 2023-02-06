using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class DailyFinishGoodReceiptParam
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public DateTime ReceiptDate { get; set; }
        public Storage Storage { get; set; }
        public Customer Customer { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public List<DailyFinishGoodReceiptDetail> Details { get; set; }
            = new List<DailyFinishGoodReceiptDetail>();
        /// <summary>
        /// Receipt in day
        /// </summary>
        public List<Receipt> Receipts { get; set; } = new List<Receipt>();
    }

    [DomainComponent]
    public class DailyFinishGoodReceiptDetail
    {
        public string PurchaseOrderNumber { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }
        public decimal OrderQuantity { get; set; }
        public decimal EntryQuantity { get; set; }
        public decimal CartonQuantity { get; set; }
        public string Remark { get; set; }
    }
}
