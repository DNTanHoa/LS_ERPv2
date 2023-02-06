using Common.Model;
using LS_ERP.BusinessLogic.Commands.Ressult;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class PullBomTypeItemStyleCommand : CommonAuditCommand,
        IRequest<PullBomTypeItemStyleResult>
    {
        public string CustomerID { get; set; }
        public string PullBomTypeCode { get; set; }
        public string Season { get; set; }
        public string SalesOrderID { get; set; }
        public IEnumerable<string> ItemStyleNumbers { get; set; }
    }
}
