using DevExpress.ExpressApp.DC;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class CalculateFabricPopupModel
    {
        public decimal? RequiredQuantity { get; set; }
        public decimal? RequiredRoll { get; set; }
    }
}
