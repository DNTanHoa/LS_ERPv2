using Common.Model;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class UpdateShippingPlanCommand
        : CommonAuditCommand, IRequest<CommonCommandResult>
    {
        public int Id { get; set; }
        public string CustomerID { get; set; }
        public string CompanyID { get; set; }
        public string FilePath { get; set; }
        public List<ShippingPlanDetail> Details { get; set; }
            = new List<ShippingPlanDetail>();
    }
}
