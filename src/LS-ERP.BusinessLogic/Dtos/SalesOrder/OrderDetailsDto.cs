using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos.SalesOrder
{
    public class OrderDetailsDto
    {
        public long ID { get; set; }
        public string ItemStyleNumber { get; set; }
        public string Size { get; set; }
        public int? SizeSortIndex { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReservedQuantity { get; set; }
        public decimal? ConsumedQuantity { get; set; }
        public string OldQuantity { get; set; }
        public decimal? Price { get; set; }
    }
}
