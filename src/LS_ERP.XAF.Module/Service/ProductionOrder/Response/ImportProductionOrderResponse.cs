using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Service.Common;
using LS_ERP.XAF.Module.Service.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class ImportProductionOrderResponse : CommonRespone
    {
        public List<ProductionOrderLineImportData> Data { get; set; }

        public new ImportProductionOrderResponse SetResult(
            bool IsSuccess, string Message)
        {
            this.Result = new CommonResult(IsSuccess, Message);
            return this;
        }
    }

    public class ProductionOrderLineImportData : ProductionOrderLine
    {
        public bool IsHasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
