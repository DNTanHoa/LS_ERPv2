using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class MonthDailyTargetDetailDtos
    {
        public MonthDailyTargetDetailDtos()
        {
            TotalDateDailyTargetDetail = new List<DateDailyTargetDetailDtos>();
        }
        public List<DateDailyTargetDetailDtos> TotalDateDailyTargetDetail { get; set; }
    }
}
