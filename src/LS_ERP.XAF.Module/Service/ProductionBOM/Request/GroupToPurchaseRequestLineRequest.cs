using LS_ERP.XAF.Module.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class GroupToPurchaseRequestLineRequest
    {
        public string PurchaseRequestID { get; set; }
        public List<ProductionBOMDto> ProductionBOMs { get; set; }
        public List<PurchaseRequestLineDto> PurchaseRequestLines { get; set; }
        public List<PurchaseRequestGroupLineDto> PurchaseRequestGroupLines { get; set; }
    }
}
