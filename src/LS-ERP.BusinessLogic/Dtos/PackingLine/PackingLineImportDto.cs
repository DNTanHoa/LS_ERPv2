using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos.PackingLine
{
    public class PackingLineImportDto
    {
        public string SequenceNo { get; set; }
        public string LSStyle { get; set; }
        public decimal? QuantitySize { get; set; } 
        public decimal? QuantityPerPackage { get; set; }
        public decimal? PackagesPerBox { get; set; }
        public decimal? QuantityPerCarton { get; set; } 
        public decimal? TotalQuantity { get; set; } 
        public decimal? NetWeight { get; set; }
        public decimal? GrossWeight { get; set; }
        public string PrePack { get; set; } 
        public string Size { get; set; }
        public decimal? Quantity { get; set; }
        public string BoxDimensionCode { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public int? FromNo { get; set; }
        public int? ToNo { get; set; }
        public int? TotalCarton { get; set; }
        public decimal? SummaryQuantity { get; set; }
        public string Note { get; set; }    
    }
}
