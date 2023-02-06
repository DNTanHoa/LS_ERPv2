using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Response
{
    public class ImportShippingPlanResponse 
    {
        public ImportShippingPlanResponse SetResult(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
            return this;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string FilePath { get; set; }
        public List<ShippingPlanDetail> Data { get; set; } = new List<ShippingPlanDetail>();
    }
}
