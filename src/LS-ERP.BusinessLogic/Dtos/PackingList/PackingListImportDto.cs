using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos.PackingList
{
    public class PackingListImportDto
    {
        public string PONumber { get; set; } = string.Empty;
        public string JDPONumber { get; set; } = string.Empty;
        public int? TotalCarton { get; set; }
        public string POSize { get; set; } = string.Empty;
        public int? POOrder { get; set; }
        public int? QuantityPerCarton { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? CartonLength { get; set; }
        public decimal? CartonWidth { get; set; }
        public decimal? CartonHeight { get; set; }
        public string NetWeight { get; set; } = string.Empty;
        public string GrossWeight { get; set; } = string.Empty;
        public int? Index { get; set; }
        public ItemStyle ItemStyle { get; set; }
        public decimal? TotalNetWeight { get; set; }
        public decimal? TotalGrossWeight { get; set; }
        public PackingUnit PackingUnit { get; set; }
        public string BoxDemensionCode { get; set; } = string.Empty;    
    }
}
