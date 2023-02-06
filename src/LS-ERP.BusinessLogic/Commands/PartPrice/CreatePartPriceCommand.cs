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
    public class CreatePartPriceCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<PartPrice>>
    {
        public int ID { get; set; }
        public string StyleNO { get; set; }
        public decimal SMV { get; set; }
        public string Item { get; set; }
        public string CompanyID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Remark { get; set; }
        
    }
}
