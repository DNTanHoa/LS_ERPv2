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
    public class GroupToIssuedLineCommand : CommonAuditCommand,
        IRequest<GroupToIssuedLineResult>
    {
        public string IssuedNumber { get; set; }
        public List<ProductionBOM> ProductionBOMs { get; set; }
        public List<IssuedLine> IssuedLines { get; set; }
        public List<IssuedGroupLine> IssuedGroupLines { get; set; }
    }
}
