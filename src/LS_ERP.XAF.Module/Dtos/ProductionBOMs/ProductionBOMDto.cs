using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos
{
    public class ProductionBOMDto
    {
        public long ID { get; set; }
        public string ItemStyleNumber { get; set; }
        public string PartMaterialID { get; set; }
        public string ExternalCode { get; set; }
        public string VendorID { get; set; }

        /// <summary>
        /// Item information
        /// </summary>
        public string DsmItemID { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }
        public string MaterialTypeCode { get; set; }
        public string MaterialTypeClass { get; set; }
        public string Position { get; set; }

        /// <summary>
        /// Garment information
        /// </summary>
        public string GarmentColorName { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentSize { get; set; }

        /// <summary>
        /// Quantity information
        /// </summary>
        public decimal? FabricWeight { get; set; }
        public decimal? FabricWidth { get; set; }
        public decimal? RequiredQuantity { get; set; }
        public decimal? WastagePercent { get; set; }
        public decimal? WastageQuantity { get; set; }
        public decimal? LessPercent { get; set; }
        public decimal? LessQuantity { get; set; }
        public decimal? FreePercent { get; set; }
        public decimal? FreeQuantity { get; set; }
        public decimal? ConsumptionQuantity { get; set; }
        public decimal? QuantityPerUnit { get; set; }
        public decimal? WareHouseQuantity { get; set; }

        public decimal? ReservedQuantity { get; set; }
        public decimal? RemainQuantity { get; set; }
        public decimal? IssuedQuantity { get; set; }

        public int? LeadTime { get; set; }

        /// <summary>
        /// Unit and price
        /// </summary>
        public string PerUnitID { get; set; }
        public string PriceUnitID { get; set; }
        public decimal? Price { get; set; }
        public string CurrencyID { get; set; }

        /// <summary>
        /// Adapter
        /// </summary>
        public string JobHeadNumber { get; set; }

        public ItemStyleDto ItemStyle { get; set; }
    }
}
