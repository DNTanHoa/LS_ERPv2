﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class ProductionBOMDtos
    {
        public long ID { get; set; }
        public string ItemStyleNumber { get; set; }
        public string PartMaterialID { get; set; }
        public string ExCode { get; set; }
        public string VendorID { get; set; }

        /// <summary>
        /// Item information
        /// </summary>
        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Specify { get; set; }
        public string GarmentSize { get; set; }
        public string MaterialTypeCode { get; set; }
        public string Position { get; set; }

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
        public decimal? ConsumptionQuantity { get; set; }

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
    }
}
