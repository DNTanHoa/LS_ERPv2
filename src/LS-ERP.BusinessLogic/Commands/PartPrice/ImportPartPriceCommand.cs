using Common.Model;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class ImportPartPriceCommand : CommonAuditCommand,
        IRequest<ImportPartPriceResult>
    {
        public string CustomerID { get; set; }
        public IFormFile UpdateFile { get; set; }
    }
}
