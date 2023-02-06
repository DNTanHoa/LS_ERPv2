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
    public class ImportSaleOrderCommand : CommonAuditCommand,
        IRequest<ImportSaleOrderResult>
    {
        public string CustomerID { get; set; }
        public string BrandCode { get; set; }
        public string Style { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public IFormFile File { get; set; }
    }
}
