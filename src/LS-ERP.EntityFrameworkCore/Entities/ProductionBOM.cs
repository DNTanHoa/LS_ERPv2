using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ProductionBOM : Audit
    {
        public long ID { get; set; }
        public string ItemStyleNumber { get; set; }
        public string PartMaterialID { get; set; }
        public string ExternalCode { get; set; }
        public string VendorID { get; set; }

        /// <summary>
        /// Item information
        /// </summary>
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }
        public string GarmentSize { get; set; }
        public string MaterialTypeCode { get; set; }
        public string MaterialTypeClass { get; set; }
        public string Position { get; set; }
        public string ContractNo { get; set; }

        /// <summary>
        /// Ghi lại thông tin số lượng sản xuất
        /// Khi thay đổi số lượng sản xuất thì tính lại required
        /// </summary>
        public decimal ProductionQuantity { get; set; }

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
        public decimal? CutWidth { get; set; }

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

        /// <summary>
        /// Item info
        /// </summary>
        public bool? GroupSize { get; set; }
        public bool? GroupItemColor { get; set; }
        public int? SizeSortIndex { get; set; }
        public string OtherName { get; set; }
        public string DefaultThreadName { get; set; }

        /// <summary>
        /// Item master information
        /// </summary>
        public string ItemMasterID { get; set; }

        public string ToDictionaryKey(string customerID)
        {
            string key = string.Empty;

            switch (customerID)
            {
                case "PU":
                    {
                        key = this.ContractNo + this.ItemID + this.ItemColorCode
                            + this.Specify;
                    }
                    break;
                default:
                    {

                    }
                    break;
            }

            return key;
        }

        [JsonIgnore]
        public virtual Unit PerUnit { get; set; }
        [JsonIgnore]
        public virtual Unit PriceUnit { get; set; }
        [JsonIgnore]
        public virtual PartMaterial PartMaterial { get; set; }
        [JsonIgnore]
        public virtual MaterialType MaterialType { get; set; }
        [JsonIgnore]
        public virtual Currency Currency { get; set; }
        [JsonIgnore]
        public virtual JobHead JobHead { get; set; }
        [JsonIgnore]
        public virtual Vendor Vendor { get; set; }

        public virtual ItemStyle ItemStyle { get; set; }

        public virtual List<ReservationEntry> ReservationEntries { get; set; }
    }
}
