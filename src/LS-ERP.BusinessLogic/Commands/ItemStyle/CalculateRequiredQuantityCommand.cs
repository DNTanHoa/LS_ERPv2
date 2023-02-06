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
    public class CalculateRequiredQuantityCommand : CommonAuditCommand,
        IRequest<CalculateRequiredQuantityResult>
    {
        public string CustomerID { get; set; }
        public List<string> StyleNumbers { get; set; }
    }
}
