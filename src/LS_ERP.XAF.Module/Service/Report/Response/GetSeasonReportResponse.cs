using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class GetSeasonReportResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<SeasonReportDetail> Data { get; set; }
    }
}
