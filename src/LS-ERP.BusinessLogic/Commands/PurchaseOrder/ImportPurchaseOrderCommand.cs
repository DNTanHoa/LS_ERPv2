using DevExpress.ExpressApp.DC;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class ImportPurchaseOrderCommand : CommonAuditCommand,
        IRequest<ImportPurchaseOrderResult>
    {
        public string CustomerID { get; set; }
        public string Season { get; set; }
        public IFormFile File { get; set; }
        public TypeImportPurchaseOrder Type { get; set; }
    }
}
