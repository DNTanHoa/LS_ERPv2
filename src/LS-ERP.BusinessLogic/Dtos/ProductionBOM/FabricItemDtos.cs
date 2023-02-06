using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class FabricItemDtos
    {
        public string ExternalCode { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string ItemMasterID { get; set; }
        public decimal FabricWidth { get; set; }
        public decimal QuantityPerUnit { get; set; }
        public string ColorCode { get; set; }
        public string CustomerID { get; set; }
        public string PerUnitID { get; set; }
        public decimal FabricWeight { get; set; }
    }
}
