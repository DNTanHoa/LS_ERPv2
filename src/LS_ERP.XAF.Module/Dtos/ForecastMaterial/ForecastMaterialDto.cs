using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos
{
    public class ForecastMaterialDto
    {
        public long ID { get; set; }
        public string ExternalCode { get; set; }
        public long? PartMaterialID { get; set; }
        public string ForecastOverallID { get; set; }

        /// <summary>
        /// Item Information
        /// </summary>
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Specify { get; set; }

        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string MaterialTypeCode { get; set; }
        public string MaterialClassType { get; set; }

        public string CurrencyID { get; set; }
        public string PerUnitID { get; set; }
        public string PriceUnitID { get; set; }
        public string VendorID { get; set; }

        /// <summary>
        /// Quantity information
        /// </summary>
        public decimal? Price { get; set; }
        public decimal? FabricWeight { get; set; }
        public decimal? FabricWidth { get; set; }
        public decimal? CutWidth { get; set; }
        public decimal? RequiredQuantity { get; set; }
        public decimal? WastagePercent { get; set; }
        public decimal? WastageQuantity { get; set; }
        public decimal? LessPercent { get; set; }
        public decimal? LessQuantity { get; set; }
        public decimal? FreePercent { get; set; }
        public decimal? FreeQuantity { get; set; }
        public decimal? ConsumptionQuantity { get; set; }
        public decimal? QuantityPerUnit { get; set; }

        public decimal? ReservedQuantity { get; set; }
        public decimal? WareHouseQuantity { get; set; }

        public int? LeadTime { get; set; }

        public ForecastOverallDto ForecastOverall { get; set; }
    }
}
