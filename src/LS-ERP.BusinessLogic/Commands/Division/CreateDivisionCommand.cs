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
    public class CreateDivisionCommand : CommonAuditCommand, 
        IRequest<CommonCommandResult<Division>>
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
