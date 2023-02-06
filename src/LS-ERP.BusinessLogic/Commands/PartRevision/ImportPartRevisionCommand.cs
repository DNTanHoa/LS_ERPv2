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
    public class ImportPartRevisionCommand : CommonAuditCommand,
       IRequest<ImportPartRevisionResult>
    {
        public string StyleNumber { get; set; }
        public string CustomerID { get; set; }
        public string Season { get; set; }
        public string RevisionNumber { get; set; }
        public DateTime? EffectDate { get; set; }
        public bool? IsConfirmed { get; set; }

        public IFormFile ImportFile { get; set; }
        public PartRevision PartRevision { get; set; }
        public List<Item> Items { get; set; }

    }
}
