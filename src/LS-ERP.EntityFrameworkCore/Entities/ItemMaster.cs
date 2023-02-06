using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ItemMaster : Audit
    {
        public ItemMaster()
        {
            ID = Guid.NewGuid().ToString().ToUpper();
        }
        public string ID { get; set; }
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
        public string Specify { get; set; }
        public string Season { get; set; }

        /// <summary>
        /// Garment information
        /// </summary>
        public string GarmentSize { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }

        /// <summary>
        /// Other information
        /// </summary>
        public bool? GroupSize { get; set; }
        public bool? GroupItemColor { get; set; }
        public string OtherName { get; set; }
        public string MaterialTypeCode { get; set; }
        public string CustomerID { get; set; }
    }
}
