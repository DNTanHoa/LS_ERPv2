using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class InventoryFG : Audit
    {
        public long ID { get; set; }
        /// <summary>
        /// public long InventoryPeriodID { get; set; }
        /// </summary>
        public string CustomerStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }
        public string Description { get; set; }
        public decimal? OnHandQuantity { get; set; }
        public string UnitID { get; set; }
        public decimal? UnitPrice { get; set; }
        public string CompanyCode { get; set; }
        [JsonIgnore]
        public virtual IList<FinishGoodTransaction> FinishGoodTransactions { get; set; }
        [JsonIgnore]
        public virtual IList<InventoryDetailFG> InventoryDetailFG { get; set; }
                    
    }
}
