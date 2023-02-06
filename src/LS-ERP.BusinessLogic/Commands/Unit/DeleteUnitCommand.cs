using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit = LS_ERP.EntityFrameworkCore.Entities.Unit;

namespace LS_ERP.BusinessLogic.Commands
{
    public class DeleteUnitCommand 
        : IRequest<CommonCommandResult<Unit>>
    {
        public string ID { get; set; }
    }
}
