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
    public class GetFabricRequestsCommand: CommonAuditCommand,
         IRequest<GetFabricRequestsResult>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string CompanyCode { get; set; }
        public string CustomerID { get; set; }
    }
}
