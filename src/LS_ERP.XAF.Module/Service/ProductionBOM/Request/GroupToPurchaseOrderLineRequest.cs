using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class GroupToPurchaseOrderLineRequest
    {
        public string PurchaseOrderID { get; set; }
        public List<ProductionBOMDto> ProductionBOMs { get; set; }
        public List<PurchaseOrderLineDto> PurchaseOrderLines { get; set; }
        public List<PurchaseOrderGroupLineDto> PurchaseOrderGroupLines { get; set; }
    }
}
