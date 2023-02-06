using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ForecastOverall : Audit
    {
        public ForecastOverall()
        {
            this.ID = Nanoid.Nanoid.Generate("123456789ABCDEFGHIJKLMNOPRST", 15);
        }

        public string ID { get; set; }
        public long? ForecastEntryID { get; set; }
        
        /// <summary>
        /// Internal information
        /// </summary>
        public string LSCode { get; set; }

        /// <summary>
        /// Garment information
        /// </summary>
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string Zone { get; set; }
        public string Division { get; set; }
        public string Brand { get; set; }
        public string Season { get; set; }
        public string LabelCode { get; set; }
        public string LabelName { get; set; }

        /// <summary>
        /// Customer information
        /// </summary>
        public string PurchaseOrderNumber { get; set; }
        public string CustomerStyle { get; set; }
        public string Description { get; set; }
        public string ContractNo { get; set; }
        public string Distribution { get; set; }
        public string FabricContent { get; set; }

        /// <summary>
        /// Forecast information
        /// </summary>
        public string CreateWeekID { get; set; }
        public string CreateWeekTitle { get; set; }
        public string ForecastWeekID { get; set; }
        public string ForecastWeekTitle { get; set; }

        /// <summary>
        /// Shipping information
        /// </summary>
        public string ShipTo { get; set; }
        public string ShippingMethod { get; set; }
        public string ShipPack { get; set; }
        public string PackRatio { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? ShipDate { get; set; }

        /// <summary>
        /// Quantity information
        /// </summary>
        public decimal? PlannedQuantity { get; set; }

        public bool? IsBomPulled { get; set; }
        public bool? IsQuantityCalculated { get; set; }
        public bool? IsQuantityBalanced { get; set; }

        public string FileName { get; set; }
        public string SaveFilePath { get; set; }

        public virtual ForecastEntry ForecastEntry { get; set; }
        public virtual Week CreateWeek { get; set; }
        public virtual Week ForecastWeek { get; set; }

        public virtual IList<ForecastDetail> ForecastDetails { get; set; }
        public virtual IList<ForecastMaterial> ForecastMaterials { get; set; }
    }
}
