using LS_ERP.BusinessLogic.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class DeleteIssuedCommand : CommonAuditCommand,
        IRequest<CommonCommandResult<object>>
    {
        public string IssuedNumber { get; set; }
    }
}
