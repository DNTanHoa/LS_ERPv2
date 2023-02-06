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
    public class CreatePartRevisionCommand : CommonAuditCommand,
        IRequest<CommonCommandResult<PartRevision>>
    {
        public string PartNumber { get; set; }
        public string RevisionNumber { get; set; }
        public DateTime? EffectDate { get; set; }
        public bool? IsConfirmed { get; set; }
        public string Season { get; set; }
        public string CustomerID { get; set; }

        public string FileName { get; set; }
        public string FileNameServer { get; set; }
        public string FilePath { get; set; }

        public List<PartMaterial> PartMaterials { get; set; }
        public List<Item> Items { get; set; }
    }
}
