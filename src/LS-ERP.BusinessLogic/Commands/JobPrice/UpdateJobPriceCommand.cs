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
    public class UpdateJobPriceCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<JobPrice>>
    {
        public int ID { get; set; }
        public string CompanyID { get; set; }
        public string CustomerID { get; set; }
        public decimal Price { get; set; }
        public string Operation { get; set; }
        public string Remark { get; set; }

    }
}
