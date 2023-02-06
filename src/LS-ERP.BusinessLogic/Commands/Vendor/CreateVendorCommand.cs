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
    public class CreateVendorCommand : CommonAuditCommand,
        IRequest<CommonCommandResult<Vendor>>
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string CurrencyID { get; set; }

    }
}
