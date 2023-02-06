using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class PartMaterialDtos
    {
        public long ID { get; set; }

        /// <summary>
        /// Part information
        /// </summary>
        public string PartNumber { get; set; }
        public long? PartRevisionID { get; set; }
        public string ExternalCode { get; set; }
        public string PartMaterialStatusCode { get; set; }

        /// <summary>
        /// Item information
        /// </summary>
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string ItemStyleNumber { get; set; }
        public string Position { get; set; }
        public string LabelCode { get; set; }
        public string LabelName { get; set; }
        public string MaterialTypeCode { get; set; }
        public string Specify { get; set; }

        /// <summary>
        /// Garment information
        /// </summary>
        public string GarmentSize { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }

        /// <summary>
        /// Purchase information
        /// </summary>
        public string VendorID { get; set; }
        public string PerUnitID { get; set; }
        public string PriceUnitID { get; set; }
        public string CurrencyID { get; set; }
        public decimal? Price { get; set; }
        public decimal? QuantityPerUnit { get; set; }
        public int? LeadTime { get; set; }

        /// <summary>
        /// Percent information
        /// </summary>
        public decimal? LessPercent { get; set; }
        public decimal? FreePercent { get; set; }
        public decimal? WastagePercent { get; set; }
        public decimal? OverPercent { get; set; }

        /// <summary>
        /// Frabric information
        /// </summary>
        public decimal? FabricWeight { get; set; }
        public decimal? FabricWidth { get; set; }
        public decimal? CutWidth { get; set; }
    }
}
