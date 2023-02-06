using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Invoice.Response
{
    public class UpdateInvoiceResponse : CommonRespone
    {
        public LS_ERP.EntityFrameworkCore.Entities.Invoice Data { get; set; }

        public new UpdateInvoiceResponse SetResult(bool isSucess, string message)
        {
            this.Result = new Common.CommonResult(isSucess, message);
            return this;
        }

    }
}
