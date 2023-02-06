using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Response
{
    public class GetIssuedReportResponse : CommonRespone
    {
        public new GetIssuedReportResponse SetResult(bool result, string message)
        {
            this.Result = new CommonResult(result, message);
            return this;
        }
        public List<IssuedReportDetail> Data { get; set; }
        = new List<IssuedReportDetail>();
    }
}
