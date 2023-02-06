using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class GroupToPurchaseOrderLineCommand : CommonAuditCommand,
        IRequest<GroupToPurchaseOrderLineResult>
    {
        public string PurchaserOrderID { get; set; }
        public List<ProductionBOM> ProductionBOMs { get; set; }
        public List<PurchaseOrderLine> PurchaseOrderLines { get; set; }
        public List<PurchaseOrderGroupLine> PurchaseOrderGroupLines { get; set; }
    }
    
}
