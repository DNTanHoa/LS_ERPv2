using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class DailyTargetSummarySearchModel
    {
        public string CompanyId { get; set; } = string.Empty;
        public DateTime ProduceDate { get; set; }
        public List<string> DepartmentIds { get; set; }
    }
}
