using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Response
{
    public class ImportForecastGroupResponse : CommonRespone
    {
        public List<ForecastOverall> Data { get; set; }

        public new ImportForecastGroupResponse SetResult(bool IsSuccess, string Message)
        {
            this.Result = new CommonResult(IsSuccess, Message);
            return this;
        }
    }
}
