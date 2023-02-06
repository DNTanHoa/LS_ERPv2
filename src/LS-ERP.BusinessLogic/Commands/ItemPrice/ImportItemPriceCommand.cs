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
    public class ImportItemPriceCommand : CommonAuditCommand,
        IRequest<CommonCommandResult<List<ItemPrice>>>
    {
        public string CustomerID { get; set; }
        public IFormFile ImportFile { get; set; }
    }
}
