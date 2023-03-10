using Common.Model;
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
    public class ImportLoadingPlanCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<List<LoadingPlan>>>
    {
        public string CustomerID { get; set; }
        public IFormFile File { get; set; }
    }
}
