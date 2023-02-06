using LS_ERP.BusinessLogic.Commands.Ressult;
using LS_ERP.BusinessLogic.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class PullBOMItemStyleCommand : CommonAuditCommand,
        IRequest<PullBOMItemStyleResult>
    {
        public string CustomerID { get; set; }
        public IEnumerable<string> ItemStyleNumbers { get; set; }
    }
}
