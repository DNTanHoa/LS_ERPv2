using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class OrderDetail
    {
        public long ID { get; set; }
        public string ItemStyleNumber { get; set; }
        public string Size { get; set; }
        public int? SizeSortIndex { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReservedQuantity { get; set; }
        public decimal? ConsumedQuantity { get; set; }
        public decimal? OldQuantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? ShipQuantity { get; set; }
        public virtual ItemStyle ItemStyle { get; set; }

        /// <summary>
        /// Số lượng mẫu, phần trăm hao hụt
        /// </summary>
        public decimal SampleQuantity { get; set; }
        public decimal OverPercent { get; set; }

        /// <summary>
        /// Invoice DE
        /// </summary>
        public decimal? OtherPrice { get; set; }

        public virtual IList<ReservationEntry> ReservationEntries { get; set; }
    }
}
