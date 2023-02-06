using DevExpress.ExpressApp.DC;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ItemStyleMaterial
    {
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string ItemCode { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemColorCode { get; set; }
        public string ItemColorName { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public string Specify { get; set; }
        public string Position { get; set; }
        public string MaterialType { get; set; }
        public string UnitID { get; set; }
        public decimal RequireQuantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal RemainQuantity { get; set; }
    }
}
