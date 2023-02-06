using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class DailyTagetOverviewDtos
    {
        public DailyTagetOverviewDtos()
        {
            Efficiency = 0;
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string StyleNO { get; set; }
        public string Item { get; set; }
        public string WorkCenterID { get; set; }
        public string WorkCenterName { get; set; }
        public decimal TotalTargetQuantity { get; set; }
        public decimal TotalOutputQuantity { get; set; }
        public int NumberOfWorker { get; set; }
        public DateTime ProduceDate { get; set; }
        public DateTime InlineDate { get; set; }
        public string Operation { get; set; }
        public string EfficiencyStr { get; set; }
        public decimal Efficiency { get; set; }
        public  DateTime? LastUpdateAt {get;set;}
        public string Status { get; set; }
        public string Problem { get; set; }
    }
}

