using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class DailyTargetDetailSummaryDtos
    {
        public string ProduceDate { get; set; }
        public int NumberOfWorker { get; set; }
        public decimal Quantity { get; set; }
        public decimal AvgEfficiency { get; set; }      
        public int Lines { get; set; }

    }
}
