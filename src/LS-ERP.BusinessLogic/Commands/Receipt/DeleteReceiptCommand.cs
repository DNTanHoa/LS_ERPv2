using Common.Model;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonAuditCommand = Common.Model.CommonAuditCommand;

namespace LS_ERP.BusinessLogic.Commands
{
    public class DeleteReceiptCommand : CommonAuditCommand,
        IRequest<CommonCommandResult>
    {
        public string ReceiptNumber { get; set; }
    }
}
