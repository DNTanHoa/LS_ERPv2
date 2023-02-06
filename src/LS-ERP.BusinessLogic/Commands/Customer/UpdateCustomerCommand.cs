using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class UpdateCustomerCommand : CommonAuditCommand,
        IRequest<CommonCommandResult<Customer>>
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PaymentTermCode { get; set; }
        public string PriceTermCode { get; set; }
        public string DivisionID { get; set; }
        public string CurrencyID { get; set; }
    }
}
