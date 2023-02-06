using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SalesOrder = LS_ERP.EntityFrameworkCore.Entities.SalesOrder;

namespace LS_ERP.BusinessLogic.Commands
{
    public class CreateSalesOrderCommand : CommonAuditCommand,
        IRequest<CommonCommandResult<SalesOrder>>
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
    }
}
