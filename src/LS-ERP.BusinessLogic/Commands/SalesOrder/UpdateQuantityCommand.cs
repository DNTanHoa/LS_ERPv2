using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class UpdateQuantityCommand : CommonAuditCommand,
        IRequest<UpdateQuantityResult>
    {
        public string CustomerID { get; set; }
        public IFormFile UpdateFile { get; set; }
    }
}
