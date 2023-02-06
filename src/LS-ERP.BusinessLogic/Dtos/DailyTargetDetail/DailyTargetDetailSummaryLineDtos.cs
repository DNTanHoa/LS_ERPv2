using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class DailyTargetDetailSummaryLineDtos
    {
        public DateTime ProduceDate { get; set; }
        public int NumberOfWorker { get; set; }
        public decimal Quantity { get; set; }
        public decimal Efficiency { get; set; }
        public string WorkCenterName { get; set; }
        public int Lines { get; set; }

    }
}
