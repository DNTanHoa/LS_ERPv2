using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ReservationEntry : Audit
    {
        public long ID { get; set; }
        
        /// <summary>
        /// Reservation between order and production information
        /// </summary>
        public string JobHeadNumber { get; set; }
        public long? OrderDetailID { get; set; }
        [JsonIgnore]
        public virtual JobHead JobHead { get; set; }
        [JsonIgnore]
        public virtual OrderDetail OrderDetail { get; set; }

        /// <summary>
        /// Reservation between finish goods with stock or production bom with stock
        /// </summary>
        public long? StorageDetailID { get; set; }
        [JsonIgnore]
        public virtual StorageDetail StorageDetail { get; set; }

        public long? PurchaseRequestLineID { get; set; }
        public virtual PurchaseRequestLine PurchaseRequestLine { get; set; }

        /// <summary>
        /// Reservation between purchase order and production bom
        /// </summary>
        public long? PurchaseOrderLineID { get; set; }
        public long? ProductionBOMID { get; set; }
        [JsonIgnore]
        public virtual PurchaseOrderLine PurchaseOrderLine { get; set; }
        [JsonIgnore]
        public virtual ProductionBOM ProductionBOM { get; set; }

        public decimal? ReservedQuantity { get; set; }
        public decimal? AvailableQuantity { get; set; }
        public decimal? IssuedQuantity { get; set; }
    }
}
