using Common.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class DeleteDeliveryNoteCommand : CommonAuditCommand,
        IRequest<CommonCommandResult>
    {
        public string ID { get; set; }
    }
}
