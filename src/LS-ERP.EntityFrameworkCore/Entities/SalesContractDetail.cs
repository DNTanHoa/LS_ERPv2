using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class SalesContractDetail : Audit
    {
        public long ID { get; set; }
        public string SalesContractID { get; set; }
        public string LSStyle { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string CustomerPO { get; set; }
        public string CustomerStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public decimal? Quantity { get; set; }
        public string UltimateCode { get; set; }
        public string ProductTop { get; set; }
        public string ProductBottom { get; set; }
        public int? Year { get; set; }
        public string Brand { get; set; }
        public string Season { get; set; }
        public string Division { get; set; }
        public string UnitID { get; set; }
        public string Factory { get; set; }
        public string CountryName { get; set; }
        public string Destination { get; set; }
        public string UCustomterCode { get; set; }
        public string ContractNo { get; set; }
        public DateTime? MRQDate { get; set; }
        public DateTime? OrderPlacedDate { get; set; }
        public DateTime? FactoryDate { get; set; }
        public string ReqShipMonth { get; set; }
        public string OrderReadyDate { get; set; }
        public int? Emboss { get; set; }
        public int? Transfer { get; set; }
        public int? Screen { get; set; }
        public int? Bonding { get; set; }
        public int? Pad { get; set; }
        public int? Lazer { get; set; }
        public int? GmtLeadTime { get; set; }
        public int? MfrLeadTime { get; set; }
        public int? LongestMaterialLeadTime { get; set; }
        public int? TransitLeadTime { get; set; }
        public string ShippingMark { get; set; }
        public string Remark { get; set; }
        public string Updates { get; set; }
        public virtual SalesContract SalesContract { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
