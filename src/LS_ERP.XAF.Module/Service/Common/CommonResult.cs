using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Common
{
    public class CommonResult
    {
        public CommonResult(bool isSuccess, string Message)
        {
            IsSuccess = isSuccess;
            this.Message = Message;
        }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public string Code { get; set; }

    }
}
