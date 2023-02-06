using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class IssuedSupplierInfoDto
    {
        public long StorageDetailID { get; set; }
        public string FabricPurchaseOrderNumber { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string SupplierName { get; set; }
    }
}
