using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class SeparationPackingLine : Audit
    {
        public int ID { get; set; }
        public string SequenceNo { get; set; }
        public string LSStyle { get; set; }
        public decimal? QuantitySize { get; set; } // for style canada
        public decimal? QuantityPerPackage { get; set; }
        public decimal? PackagesPerBox { get; set; }
        public decimal? QuantityPerCarton { get; set; } // QtyPerCTNS
        public decimal? TotalQuantity { get; set; } // QtyPCS
        public decimal? NetWeight { get; set; }
        public decimal? GrossWeight { get; set; }
        public string Color { get; set; }
        public string PrePack { get; set; } // = field size of old version
        public string Size { get; set; }
        public decimal? Quantity { get; set; }
        public int SeparationPackingListID { get; set; }
        public string BoxDimensionCode { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public int? FromNo { get; set; }
        public int? ToNo { get; set; }
        public int? TotalCarton { get; set; }
        public string DeliveryPlace { get; set; }
        public virtual BoxDimension BoxDimension { get; set; }
        public virtual SeparationPackingList SeparationPackingList { get; set; }
    }
}
