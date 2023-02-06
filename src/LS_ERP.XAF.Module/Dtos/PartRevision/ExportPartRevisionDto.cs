using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos
{
    public class ExportPartRevisionDto
    {
        public string ItemNo { get; set; }
        public string Style { get; set; }
        public string ColorGarmentCode { get; set; }
        public string ColorGarmentName { get; set; }
        public string MaterialCode { get; set; }
        public string DsmCode { get; set; }
        public string Description { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string GarmentSize { get; set; }
        public bool GarmentDeben { get; set; }
        public string Specification { get; set; }
        public string Label { get; set; }
        public string Division { get; set; }
        public decimal Consumption { get; set; }
        public string UnitBOM { get; set; }
        public string Position { get; set; }
        public string MaterialClass { get; set; }
        public decimal Price { get; set; }
        public string UnitPrice { get; set; }
        public string Currency { get; set; }
        public string Vendor { get; set; }
        public int LeadTime { get; set; }
        public decimal Wastage { get; set; }
        public decimal Less { get; set; }
        public decimal Over { get; set; }
        public decimal FabricsWeight { get; set; }
        public decimal FabricsWidth { get; set; }
        public decimal FabricsWidthCut { get; set; }
        public string MaterialClassType { get; set; }
        public decimal FreePercent { get; set; }
        public string OtherName { get; set; }
    }
}
