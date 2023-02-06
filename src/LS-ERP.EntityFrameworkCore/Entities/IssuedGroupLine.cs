using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class IssuedGroupLine : Audit
    {
        public long ID { get; set; }
        public string IssuedNumber { get; set; }

        /// <summary>
        /// Item master information
        /// </summary>
        public string ItemMasterID { get; set; }

        /// <summary>
        /// Item information
        /// </summary>
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Specify { get; set; }
        public string Season { get; set; }
        public string UnitID { get; set; }

        public long StorageDetailID { get; set; } // for FB WH
        public long? FabricRequestDetailID { get; set; } // for FB WH
        public string StorageStatusID { get; set; } // for FB WH
        public string FabricPurchaseOrderNumber { get; set; } // for FB WH
        public string PurchaseOrderNumber { get; set; } // for FB WH
        /// <summary>
        /// Garment information
        /// </summary>
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string CustomerStyle { get; set; }

        public decimal? IssuedQuantity { get; set; }
        public decimal? RequestQuantity { get; set; }// for FB WH
        public decimal? ReceivedQuantity { get; set; }
        public decimal? CartonQuantity { get; set; }
        public decimal? Roll { get; set; }

        public string StorageBinCode { get; set; }
        public string LotNumber { get; set; }
        public string DyeLotNumber { get; set; }

        public virtual Issued Issued { get; set; }
        public virtual StorageStatus StorageStatus { get; set; }

        public virtual IList<IssuedLine> IssuedLines { get; set; } = new List<IssuedLine>();
    }
}
