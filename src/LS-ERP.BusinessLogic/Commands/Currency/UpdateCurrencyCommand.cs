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
    public class UpdateCurrencyCommand : CommonAuditCommand,
        IRequest<CommonCommandResult<Currency>>
    {
        public string ID { get; set; }
        public string Description { get; set; }
    }
}
