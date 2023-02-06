using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Win.Dtos
{
    public class ExportQuantity_Dto
    {
        public string ItemID { get; set; }
        public decimal? Quantity { get; set; }
        public string SupplierCnuf { get; set; }
        public string Shipment { get; set; }
        public string Incoterm { get; set; }
        public string Currency { get; set; }
        public string CustomerStyle { get; set; }
        public string Comment { get; set; }
    }
}
