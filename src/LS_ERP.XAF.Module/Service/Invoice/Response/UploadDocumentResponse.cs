using LS_ERP.XAF.Module.Dtos.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Invoice.Response
{
    public class UploadDocumentResponse : CommonRespone
    {
        public DocumentFileInfoDto Data { get; set; }

        public new UploadDocumentResponse SetResult(bool isSucess, string message)
        {
            this.Result = new Common.CommonResult(isSucess, message);
            return this;
        }
    }
}
