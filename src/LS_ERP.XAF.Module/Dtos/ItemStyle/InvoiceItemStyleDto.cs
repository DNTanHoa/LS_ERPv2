using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos
{
    public class InvoiceItemStyleDto
    {
        public string PurchaseOrderNumber { get; set; }
        public string CustomerStyle { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string LSStyle { get; set; }
        public string Size { get; set; }
    }
}
