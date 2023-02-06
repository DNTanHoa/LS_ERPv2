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
    public class GroupToPurchaseRequestLineCommand : CommonAuditCommand,
        IRequest<GroupToPurchaseRequestLineResult>
    {
        public string PurchaserRequestID { get; set; }
        public List<ProductionBOM> ProductionBOMs { get; set; }
        public List<PurchaseRequestLine> PurchaseRequestLines { get; set; }
        public List<PurchaseRequestGroupLine> PurchaseRequestGroupLines { get; set; }
    }
}
