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
    public class UpdateProblemCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<Problem>>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int SortIndex { get; set; }
    }
}
