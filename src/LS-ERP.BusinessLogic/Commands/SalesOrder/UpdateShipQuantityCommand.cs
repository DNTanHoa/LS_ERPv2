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
    public class UpdateShipQuantityCommand : CommonAuditCommand,
        IRequest<UpdateShipQuantityResult>
    {
        public string CustomerID { get; set; }
        public IFormFile UpdateFile { get; set; }
    }
}
