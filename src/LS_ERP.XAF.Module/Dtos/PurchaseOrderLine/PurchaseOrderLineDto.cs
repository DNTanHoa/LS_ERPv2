using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Dtos
{
    public class PurchaseOrderLineDto
    {
        public PurchaseOrderLineDto()
        {
            ReservationEntries = new List<ReservationEntryDto>();
        }

        public long ID { get; set; }
        public string PurchaseOrderID { get; set; }
        public string SalesOrderID { get; set; }
        public long? PurchaseOrderGroupLineID { get; set; }

        public string ItemID { get; set; }
        public string DsmItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Mfg { get; set; }
        public string UPC { get; set; }
        public string Division { get; set; }
        public string Label { get; set; }
        public string Specify { get; set; }
        public string Remarks { get; set; }

        /// <summary>
        /// garment information
        /// </summary>
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Season { get; set; }
        public string ContractNo { get; set; }
        public DateTime? ShipDate { get; set; }
        public decimal? WastageQuantity { get; set; }

        /// <summary>
        /// Price and unit
        /// </summary>
        public decimal? Price { get; set; }
        public string UnitID { get; set; }
        public string SecondUnitID { get; set; }
        public string WareHouseUnitID { get; set; }

        /// <summary>
        /// Quantity information
        /// </summary>
        public decimal? ReservedQuantity { get; set; }
        public decimal? ReservedForecastQuantity { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? QuantityPerUnit { get; set; }
        public decimal? FreePercent { get; set; }
        public decimal? FreePercentQuantity { get; set; }
        public decimal? WareHouseQuantity { get; set; }
        public decimal? CanReusedQuantity { get; set; }

        public decimal? MSRP { get; set; }
        public string SuppPlt { get; set; }
        public string ReplCode { get; set; }
        public string DeptSubFineline { get; set; }
        public string FixtureCode { get; set; }
        public string TagSticker { get; set; }
        public string ModelName { get; set; }
        public string CustomerPurchaseOrderNumber { get; set; }

        public List<ReservationEntryDto> ReservationEntries { get; set; }
        public List<ReservationForecastEntryDto> ReservationForecastEntries { get; set; }
    }
}