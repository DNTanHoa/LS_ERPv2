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
    public class UpdateSampleQuantityCommand : CommonAuditCommand,
        IRequest<UpdateSampleQuantityResult>
    {
        public string CustomerID { get; set; }
        public IFormFile UpdateFile { get; set; }
    }
}
