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
    public class ImportPartCommand : CommonAuditCommand,
       IRequest<CommonCommandResult<Part>>
    {
        public string CustomerID { get; set; }
        public string UserName { get; set; }
        public bool Update { get; set; }
        public string SalesOrderIDs { get; set; }
        public IFormFile FilePath { get; set; }
    }
}
