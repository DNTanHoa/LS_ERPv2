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
    public class SavePurchaseRequestCommand : CommonAuditCommand,
        IRequest<CommonCommandResult<PurchaseRequest>>
    {
        public string ID { get; set; }
        public string Number { get; set; }
        public string CustomerID { get; set; }
        public string DivisionID { get; set; }
        public string CurrencyID { get; set; }
        public string CurrencyExchangeTypeID { get; set; }
        public decimal? ExchangeValue { get; set; }
        public string Reason { get; set; }

        /// <summary>
        /// Time information
        /// </summary>
        public DateTime? RequestDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ConfirmDate { get; set; }

        public List<PurchaseRequestGroupLine> PurchaseRequestGroupLines { get; set; }
    }
}
