using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Response
{
    public class GetItemDailyTargetDetailReportResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<DailyTargetDetailReportParam> Data { get; set; }
    }
}
