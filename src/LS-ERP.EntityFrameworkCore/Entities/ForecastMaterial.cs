using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ForecastMaterial : Audit
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
        public decimal? RemainQuantity { get; set; }
        public decimal? WareHouseQuantity { get; set; }

        public int? LeadTime { get; set; }
        /// <summary>
        /// Item info
        /// </summary>
        public int? SizeSortIndex { get; set; }
        public string OtherName { get; set; }
        public string DefaultThreadName { get; set; }

        /// <summary>
        /// Item master information
        /// </summary>
        public string ItemMasterID { get; set; }

        public virtual Unit PerUnit { get; set; }
        public virtual Unit PriceUnit { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual MaterialType MaterialType { get; set; }

        public long? ForecastDetailID { get; set; }

        public virtual ForecastDetail ForecastDetail { get; set; }
        public virtual ForecastOverall ForecastOverall { get; set; }

        public virtual IList<ReservationForecastEntry> ReservationForecastEntries { get; set; }
    }
}
