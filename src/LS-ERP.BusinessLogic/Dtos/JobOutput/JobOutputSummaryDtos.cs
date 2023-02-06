using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class JobOutputSummaryDtos
    {
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string JobHeadNumber { get; set; }
        public string JobOperationID { get; set; }
        public string JobOperationName { get; set; }
        public DateTime? OutputAt { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? TargetQuantity { get; set; }
        public decimal? PassedQuantity { get; set; }
        public decimal? Efficiency { get; set; }
        public string   EfficiencyStr { get; set; }
        public string Problem { get; set; }
        public int NumberOfWorker { get; set; }
        public List<string> Problems { get; set; }
        public string WorkCenterID { get; set; }
        public string WorkCenterName { get; set; }
        public string DepartmentID { get; set; }
        public int WorkingTimeID { get; set; }
        public string WorkingTimeName { get; set; }
        public string StyleNO { get; set; }
        public string ItemStyleDescription { get; set; }
    }
}
