using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class IssuedFinishGoodParam
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public Customer Customer { get; set; }
        public Storage Storage { get; set; }
        public DateTime IssuedDate { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Search style, PO
        /// </summary>
        public string SearchStyle { get; set; }

        public List<IssuedFinishGoodDetail> Details { get; set; } = new List<IssuedFinishGoodDetail>();

        public List<Issued> Issueds { get; set; } = new List<Issued>();
    }

    [DomainComponent]
    public class IssuedFinishStyle
    {
        public string PurchaseOrderNumber { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string Season { get; set; }
        public string ProductDescription { get; set; }
    }

    [DomainComponent]
    public class IssuedFinishGoodDetail
    {
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string ProductionDescription { get; set; }
        public string Port { get; set; }
        public string Status { get; set; }
        public decimal InStockQuantity { get; set; }
        public decimal IssuedQuantity { get; set;  }
        public string Note { get; set; }
    }
}
