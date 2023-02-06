using LS_ERP.XAF.Module.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class MapPriceRequest
    {
        public string VendorID { get; set; }
        public string ShippingTermCode { get; set; }
        public string CustomerID { get; set; }
        public List<PurchaseOrderLineDto> PurchaseOrderLines { get; set; }
    }
}
