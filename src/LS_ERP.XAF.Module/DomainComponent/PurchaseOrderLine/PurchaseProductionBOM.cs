using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class PurchaseProductionBOM
    {
        [XafDisplayName("Vendor")]
        public string VendorID { get; set; }
        public string Style { get; set; }
        [XafDisplayName("Contract No")]
        public string ContractNumber { get; set; }
        public List<ItemStyle> ItemStyles { get; set; }
        [XafDisplayName("Production Boms")]
        public List<ProductionBOM> ProductionBOMs { get; set; }

        /// <summary>
        /// Phần trăm mua hàng
        /// </summary>
        public int Percent { get; set; }
        
        [VisibleInListView(false)]
        [VisibleInDetailView(false)]
        [VisibleInLookupListView(false)]
        public List<ReservationEntry> ReservationEntries { get; set; }
    }
}
