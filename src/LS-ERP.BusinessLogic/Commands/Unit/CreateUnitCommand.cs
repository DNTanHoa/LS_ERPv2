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
    public class CreateUnitCommand : CommonAuditCommand,
        IRequest<CommonCommandResult<Unit>>
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public bool Rouding { get; set; }
        public string DecUnit { get; set; }
        public decimal Factor { get; set; }
        public bool RoundingPurchase { get; set; }
        public bool RoundUp { get; set; }
        public bool RoundDown { get; set; }
    }
}
