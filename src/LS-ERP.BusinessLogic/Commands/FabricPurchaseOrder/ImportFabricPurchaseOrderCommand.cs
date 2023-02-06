using Common.Model;
using LS_ERP.BusinessLogic.Commands.Result;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class ImportFabricPurchaseOrderCommand : CommonAuditCommand,
         IRequest<ImportFabricPurchaseOrderResult>
    {
        public IFormFile File { get; set; }
        public string CustomerID { get; set; }
        public string ProductionMethodCode { get; set; }
    }
}
