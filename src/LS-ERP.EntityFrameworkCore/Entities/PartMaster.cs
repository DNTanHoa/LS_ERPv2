using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PartMaster : Audit
    {
        public PartMaster()
        {
            ID = Guid.NewGuid().ToString().ToUpper();
        }
        public string ID { get; set; }
        public string ExternalID { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }
        public string CustomerStyle { get; set; }
        public string CustomerID { get; set; }

        /// <summary>
        /// For Make
        /// </summary>
        public int MaxInStockQuantity { get; set; }
        public int MinInStockQuantity { get; set; }

        /// API DE
        public string ModelLib { get; set; }
        public string ModelLibProd { get; set; }
        public string Label { get; set; }
        public string EAN { get; set; }
        public string Type { get; set; }
        public int? PCB { get; set; }
        public int? UE { get; set; }
        public string Component { get; set; }
    }
}
