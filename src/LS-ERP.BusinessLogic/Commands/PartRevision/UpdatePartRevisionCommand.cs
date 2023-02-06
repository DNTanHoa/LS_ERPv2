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
    public class UpdatePartRevisionCommand : CommonAuditCommand,
        IRequest<CommonCommandResult<PartRevision>>
    {
        public long ID { get; set; }
        public string PartNumber { get; set; }
        public string RevisionNumber { get; set; }
        public DateTime? EffectDate { get; set; }
        public bool? IsConfirmed { get; set; }
        public string Season { get; set; }
    }
}
