using Common.Model;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class CreateOperationCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<Operation>>
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }      
        public int Index { get; set; }

    }
}
