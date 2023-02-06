using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class MaterialTransactionReportParam
    {
        [Browsable(false)]
        [DevExpress.ExpressApp.Data.Key]
        public int ID { get; set; }
        public Storage Storage { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<MaterialTransactionReportDetail> Details { get; set; }
    }

    [DomainComponent]
    public class MaterialTransactionReportDetail
    {
        /// <summary>
        /// Item information
        /// </summary>
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }

        /// <summary>
        /// Style information
        /// </summary>
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string Season { get; set; }

        /// <summary>
        /// Garment information
        /// </summary>
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string UnitID { get; set; }
        public string CustomerID { get; set; }
        public string StorageCode { get; set; }
        public decimal InQuantity { get; set; }
        public decimal OutQuantity { get; set; }
    }
}
