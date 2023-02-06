using LS_ERP.BusinessLogic.Commands.Result;
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
    public class MapPriceCommand : CommonAuditCommand, 
        IRequest<MapPriceResult>
    {
        public string CustomerID { get; set; }
        public string VendorID { get; set; }
        public string ShippingTermCode { get; set; }
        public List<PurchaseOrderLine> PurchaseOrderLines { get; set; }
    }
}
