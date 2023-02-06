using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LS_ERP.Identity.API.Model
{
    public class CommonResponseModel
    {
        public CommonApiResult Result { get; set; }
        public object Data { get; set; }

        public CommonResponseModel SetResult(string code, string message)
        {
            this.Result = new CommonApiResult(code, message);
            return this;
        }

        public CommonResponseModel SetData(object data)
        {
            this.Data = data;
            return this;
        }
    }
}
