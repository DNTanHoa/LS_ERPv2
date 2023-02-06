using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class BalanceQuantityParam
    {
        public Customer Customer { get; set; }
        public List<ItemStyle> ItemStyles { get; set; }
        public List<MaterialBalanceQuantity> Materials { get; set; }
        public List<PurchaseOrderLine> PurchaseForecast { get; set; }
        [VisibleInDetailView(false)]
        public List<ReservationEntry> ReservationEntries { get; set; }
    }

    [DomainComponent]
    public class MaterialBalanceQuantity
    {
        [VisibleInListView(false)]
        public long ProductionBOMID { get; set; }
        public string VendorID { get; set; }
        public string ItemID { get; set; }
        public string ItemMasterID { get; set; }

        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }

        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string Position { get; set; }
        public string Specify { get; set; }
        public string Season { get; set; }

        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string UnitID { get; set; }

        public decimal RequiredQuantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal RemainQuantity { get; set; }
        public decimal BalanceQuantity { get; set; }

        public string Status { get; set; }
    }
}
