using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PurchaseRequest : Audit
    {
        public PurchaseRequest()
        {
            this.ID = Nanoid.Nanoid.Generate("ABCDEFGHJKLMNOPQRST123456789", 15);
            this.Number = "PR" + DateTime.Today.Year + "-" + Nanoid.Nanoid.Generate("ABCDEFGHJKLMNOPQRST123456789", 4);
        }

        public string ID { get; set; }
        public string Number { get; set; }
        public string CustomerID { get; set; }
        public string DivisionID { get; set; }
        public string CurrencyID { get; set; }
        public string CurrencyExchangeTypeID { get; set; }
        public decimal? ExchangeValue { get; set; }
        public string Reason { get; set; }
        public string StatusID { get; set; }

        /// <summary>
        /// Time information
        /// </summary>
        public DateTime? RequestDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ConfirmDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Division Division { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual CurrencyExchangeType CurrencyExchangeType { get; set; }
        public virtual PurchaseRequestStatus Status { get; set; }

        public virtual IList<PurchaseRequestGroupLine> PurchaseRequestGroupLines { get; set; }
        public virtual IList<PurchaseRequestLine> PurchaseRequestLines { get; set; }
        public virtual IList<PurchaseRequestLog> Logs { get; set; }
    }
}
