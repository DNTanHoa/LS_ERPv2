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
    public class UpdateFabricContrastCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<FabricContrast>>
    {
        public int ID { get; set; }
        public string MergeBlockLSStyle { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DescriptionForShirt { get; set; }
        public string DescriptionForPant { get; set; }
        public string ContrastColor { get; set; }

    }
}
