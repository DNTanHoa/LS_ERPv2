using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service.Common;
using LS_ERP.XAF.Module.Service.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Storage
{
    public class ImportStorageResponse : CommonRespone
    {
        public List<ImportStorageData> Data { get; set; }
        public new ImportStorageResponse SetResult(bool IsSuccess, string Message)
        {
            this.Result = new CommonResult(IsSuccess, Message);
            return this;
        }
    }
}
