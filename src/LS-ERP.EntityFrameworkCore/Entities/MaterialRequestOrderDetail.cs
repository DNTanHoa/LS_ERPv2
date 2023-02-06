using NetTopologySuite.Noding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class MaterialRequestOrderDetail
    {
        public int ID { get; set; }
        public int MaterialRequestId { get; set; }
        public string ItemStyleNumber { get; set; }
        public int OrderDetailID { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string GarmentSize { get; set; }
        public int SizeSortIndex { get; set; }
        public int Quantity { get; set; }
        public int RequestQuantity { get; set; }
        public int SampleQuantity { get; set; }
        public int PercentQuantity { get; set; }
        public int TotalQuantity { get; set; }

        public virtual MaterialRequest MaterialRequest { get; set; }
    }
}
