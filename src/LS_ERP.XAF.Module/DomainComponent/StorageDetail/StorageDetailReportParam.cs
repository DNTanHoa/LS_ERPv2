using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class StorageDetailReportParam
    {
        [Browsable(false)]
        [Key]
        public int ID { get; set; }
        public Storage Storage { get; set; }
        public Customer Customer { get; set; }
        public PriceTerm ProductionMethodCode { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        [XafDisplayName("OnHandQuantity > 0")]
        public bool OnHandQuantityZero { get; set; } = true;
        public List<StorageDetailsReport> Details { get; set; } = new List<StorageDetailsReport>();
    }
}
