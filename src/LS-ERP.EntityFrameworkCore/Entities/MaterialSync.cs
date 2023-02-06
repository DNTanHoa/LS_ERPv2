using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class MaterialSync
    {
        public int ID { get; set; }
        public int ProductionBOMID { get; set; }
        public int ItemStyleSyncMasterID { get; set; }
        /// <summary>
        /// Item information
        /// </summary>
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }
        public string GarmentSize { get; set; }
        public string MaterialTypeCode { get; set; }
        public string MaterialTypeClass { get; set; }
        public string Position { get; set; }

        /// <summary>
        /// Quantity information
        /// </summary>
        public decimal RequiredQuantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal IssuedQuantity { get; set; }
        public decimal RemainQuantity { get; set; }

        /// <summary>
        /// Required date
        /// </summary>
        public DateTime? RequiredDate { get; set; }
        public DateTime? FirstIssuedDate { get; set; }
        public DateTime? LastIssuedDate { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; }

        public virtual ItemStyleSyncMaster ItemStyleSyncMaster { get; set; }
        public virtual List<MaterialSyncDetail> Details { get; set; }
    }
}
