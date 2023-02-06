using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class UpdateSalesOrderCommand : CommonAuditCommand,
         IRequest<UpdateSalesOrderResult>
    {
        public string ID { get; set; }
        public string CustomerID { get; set; }
        public string BrandCode { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public string DivisionID { get; set; }
        public string PaymentTermCode { get; set; }
        public string PriceTermCode { get; set; }
        public string CurrencyID { get; set; }
        public int? Year { get; set; }
        public string SalesOrderStatusCode { get; set; }
        public string SalesOrderOrderTypeCode { get; set; }
        public IFormFile UpdateFile { get; set; }

    }
}
