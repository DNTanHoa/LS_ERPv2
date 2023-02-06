using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class PurchaseForecastMaterial
    {
        [XafDisplayName("Vendor")]
        public string VendorID { get; set; }

        [XafDisplayName("Style")]
        public string Style { get; set; }

        [XafDisplayName("Forecast Overalls")]
        public List<ForecastOverall> ForecastOveralls { get; set; }

        [XafDisplayName("Forecast Materials")]
        public List<ForecastMaterial> ForecastMaterials { get; set; }

        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public List<ReservationForecastEntry> ReservationForecastEntries { get; set; }
    }
}
