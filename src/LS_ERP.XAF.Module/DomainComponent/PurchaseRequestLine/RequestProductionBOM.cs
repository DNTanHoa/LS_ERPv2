using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class RequestProductionBOM
    {
        [XafDisplayName("Search Style")]
        public string ItemStyle { get; set; }

        [XafDisplayName("Styles")]
        public List<ItemStyle> ItemStyles { get; set; }

        [XafDisplayName("Bom")]
        public List<ProductionBOM> ProductionBOMs { get; set; }
    }
}
