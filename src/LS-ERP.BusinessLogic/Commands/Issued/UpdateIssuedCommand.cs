using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class UpdateIssuedCommand : CommonAuditCommand,
        IRequest<CommonCommandResult<Issued>>
    {
        public string Number { get; set; }
        public string DocumentReferenceNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceNumberNoTotal { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }

        /// <summary>
        /// Date time information
        /// </summary>
        public DateTime? IssuedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }

        /// <summary>
        /// Issued information
        /// </summary>
        public string IssuedBy { get; set; }
        public string ReceivedBy { get; set; }

        /// <summary>
        /// Finance information
        /// </summary>
        public string AccountDebitNumber { get; set; }
        public string AccountCreditNumber { get; set; }

        /// <summary>
        /// Addtional information
        /// </summary>
        public string CustomerID { get; set; }
        public string StorageCode { get; set; }

        public List<IssuedGroupLineDto> IssuedGroupLines { get; set; }
        public List<IssuedLineDto> IssuedLines { get; set; }
    }
}
