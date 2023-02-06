using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ReservationForecastEntry : Audit
    {
        public long ID { get; set; }

        public long? PurchaseOrderLineID { get; set; }
        public long? ForecastMaterialID { get; set; }
        [JsonIgnore]
        public virtual PurchaseOrderLine PurchaseOrderLine { get; set; }
        [JsonIgnore]
        public virtual ForecastMaterial ForecastMaterial { get; set; }

        public decimal? ReservedQuantity { get; set; }
        public decimal? AvailableQuantity { get; set; }
    }
}
