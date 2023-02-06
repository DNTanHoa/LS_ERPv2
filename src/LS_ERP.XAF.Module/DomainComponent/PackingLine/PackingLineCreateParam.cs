using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class PackingLineCreateParam
    {
        public List<OrderDetailForPacking> OrderDetails { get; set; }
        public List<PackingRatio> PackingRatios { get; set; }
        [VisibleInDetailView(false)]
        public List<PackingOverQuantity> PackingOverQuantities { get; set; }
        public int TotalQuantity { get; set; }
        public int QuantityPackagePerCarton { get; set; }
        public bool RemainQuantity { get; set; }
        public PackingType PackingType { get; set; }
    }

    [DomainComponent]
    public class OrderDetailForPacking
    {
        [VisibleInListView(false)]
        public long OrderDetailID { get; set; }
        [XafDisplayName("Color Code")]
        public string ColorCode { get; set; }
        [XafDisplayName("Color Name")]
        public string ColorName { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public int BalanceQuantity { get; set; }
        public int ShipQuantity { get; set; }
        public int? NoCartonShortShip { get; set; }

        [VisibleInListView(false)]
        public string LSStyle { get; set; }
        [VisibleInListView(false)]
        public string DeliveryPlace { get; set; }
        [VisibleInListView(false)]
        public string ItemStyleNumber { get; set; }
        [VisibleInListView(false)]
        public int? SizeSortIndex { get; set; }
        [VisibleInListView(false)]
        public bool? MultiShip { get; set; }
        [VisibleInListView(false)]
        public string CustomerStyle { get; set; }
    }

    [DomainComponent]
    public class PackingRatio
    {
        public ItemStyle Color { get; set; }
        public string Size { get; set; }
        public int Ratio { get; set; }
        public int TotalQuantity { get; set; }
        [VisibleInListView(false)]
        public int? SizeSortIndex { get; set; }
        public int TotalCarton { get; set; }
    }

    public enum PackingType
    {
        [XafDisplayName("Solid size - Solid color")]
        SolidSizeSolidColor,
        [XafDisplayName("Solid size - Assorted color")]
        SolidSizeAssortedColor,
        [XafDisplayName("Assorted size - Solid color")]
        AssortedSizeSolidColor,
        //[XafDisplayName("Assorted size - Assorted color")]
        //AssortedSizeAssortedColor
    }
}
