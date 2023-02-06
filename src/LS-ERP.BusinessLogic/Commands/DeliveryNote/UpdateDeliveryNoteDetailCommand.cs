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
    public class UpdateDeliveryNoteDetailCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<DeliveryNoteDetail>>
    {
        public List<string> Ids { get; set; }        

    }
}
