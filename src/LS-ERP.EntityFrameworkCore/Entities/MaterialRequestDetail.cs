using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class MaterialRequestDetail
    {
        public int Id { get; set; }
        public int MaterialRequestId { get; set; }

        /// <summary>
        /// Thông tin item
        /// </summary>
        public string ItemMaterID { get; set; }
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }
        public string Position { get; set; }

        /// <summary>
        /// Garment infor
        /// </summary>
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }

        /// <summary>
        /// Quantity infor
        /// </summary>
        public decimal RequiredQuantity { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityPerUnit { get; set; }

        /// <summary>
        /// For only fabric
        /// </summary>
        public decimal Roll { get; set; }
        public string UnitID { get; set; }
        public string Remarks { get; set; }

        /// <summary>
        /// Group size material request
        /// </summary>
        public bool GroupSize { get; set; }
        public bool GroupItemColor { get; set; }
        public string OtherName { get; set; }

        public string DefaultThreadName { get; set; }

        public virtual MaterialRequest MaterialRequest { get; set; }
    }
}
