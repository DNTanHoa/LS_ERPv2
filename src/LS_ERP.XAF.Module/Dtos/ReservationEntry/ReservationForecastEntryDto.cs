using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos
{
    public class ReservationForecastEntryDto
    {
        public long ID { get; set; }

        public long? PurchaseOrderLineID { get; set; }
        public long? ForecastMaterialID { get; set; }
        public decimal? ReservedQuantity { get; set; }
        public decimal? AvailableQuantity { get; set; }
    }
}
