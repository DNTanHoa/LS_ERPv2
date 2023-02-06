using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos
{
    public class OutputDailyTargetDetailByCustomerDtos
    {
        public string CustomerName { get; set; } = string.Empty;
        public string WorkCenterName { get; set; } = string.Empty;
        public decimal Efficiency { get; set; }
        public decimal Quantity { get; set; }
        public int Lines { get; set; }
    }
}
