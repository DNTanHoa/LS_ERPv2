using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class DailyTargetImportParam
    {
        [ImmediatePostData(true)]
        public string ImportFilePath { get; set; }
        public List<ImportDailyTargetData> Data { get; set; } = new List<ImportDailyTargetData>();

    }
    [DomainComponent]
    public class ImportDailyTargetData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string StyleNO { get; set; }
        public string Item { get; set; }
        public string WorkCenterID { get; set; }
        public string WorkCenterName { get; set; }
        public decimal TotalTargetQuantity { get; set; }
        public decimal SMV { get; set; }
        public int NumberOfWorker { get; set; }
        public DateTime ProduceDate { get; set; }
        public DateTime InlineDate { get; set; }
        public string Operation { get; set; }
    }
}
