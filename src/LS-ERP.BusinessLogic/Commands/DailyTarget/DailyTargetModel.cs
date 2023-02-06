using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class DailyTargetModel
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
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string GarmentColor { get; set; }
        public string LSStyle { get; set; }
        public bool IsCreatedDetail { get; set; }
        public bool IsCanceled { get; set; }
        public string Remark { get; set; }
        public decimal Sample { get; set; }
        public string SampleWithSize { get; set; }
        public decimal Percent { get; set; }
        public decimal PercentPrint { get; set; }
        public string FabricContrast { get; set; }
        public string CompanyID { get; set; }
        public int Week { get; set; }
        public string MergeLSStyle { get; set; }
        public string MergeBlockLSStyle { get; set; }
        public DateTime PSDD { get; set; }
        public decimal TargetQuantity { get; set; }
        public string Set { get; set; }
        public string Size { get; set; }
        public string UserName { get; set; }
        public string OrderTypeName { get; set; }
    }
}

