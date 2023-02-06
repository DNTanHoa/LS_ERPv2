using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class ImportStorageBinEntryDto
    {
        public string PurchaseOrderNumber { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string Season { get; set; }
        public string BinCode { get; set; }
    }
}
