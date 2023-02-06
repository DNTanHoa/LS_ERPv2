using DevExpress.ExpressApp.DC;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class JobOutputDetail
    {
        public string LSStyle { get; set; }
        public string CustomerStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string GarmentSize { get; set; }
        public decimal RequiredQuantity { get; set; }
        public decimal OutputQuantity { get; set; }
    }
}
