using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class MergeItemCommand : CommonAuditCommand,
         IRequest<MergeItemResult>
    {
        public string CustomerID { get; set; }
        public List<string> PartRevisionIDs { get; set; }
    }
}
