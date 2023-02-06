using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class FinishGoodBinEntryParam
    {
        public Customer Customer { get; set; }
        public Storage Storage { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string Style { get; set; }

        public List<StyleFinishGoodBinEntry> Entries { get; set; } = new List<StyleFinishGoodBinEntry>();
    }

    [DomainComponent]
    public class StyleFinishGoodBinEntry
    {
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string Season { get; set; }
        public string BinCode { get; set; }
    }
}
