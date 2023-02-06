using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LS_ERP.Identity.API.Model
{
    public class CommonApiResult
    {
        public CommonApiResult(string code, string message)
        {
            Code = code;
            Message = message;
        }
        public string Code { get; set; }
        public string Message { get; set; }
    }
}
