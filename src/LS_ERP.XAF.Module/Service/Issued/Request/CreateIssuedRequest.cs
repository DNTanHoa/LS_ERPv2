using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class CreateIssuedRequest
    {
        public string UserName { get; set; }
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
        public string ProductionMethodCode { get; set; }

        public long FabricRequestID { get; set; }

        public List<IssuedGroupLineDto> IssuedGroupLines { get; set; }
    }
}
