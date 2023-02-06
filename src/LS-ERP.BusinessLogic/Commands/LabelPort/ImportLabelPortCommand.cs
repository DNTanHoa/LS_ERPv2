using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class ImportLabelPortCommand : CommonAuditCommand,
       IRequest<ImportLabelPortResult>
    {
        public string CustomerID { get; set; }
        public IFormFile FilePath { get; set; }
    }
}
