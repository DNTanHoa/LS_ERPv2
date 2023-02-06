using Common.Model;
using LS_ERP.BusinessLogic.Commands.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class UpdateStatusFabricRequestCommand : CommonAuditCommand,
         IRequest<UpdateStatusFabricRequestResult>
    {
        public long ID { get; set; }
        public string StatusID { get; set; }
        public string Remark { get; set; }
    }
}
