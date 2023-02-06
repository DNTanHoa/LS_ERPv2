using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ItemPriceImportParam
    {
        public Customer Customer { get; set; }
        [XafDisplayName("File")]
        public string FilePath { get; set; }
        [XafDisplayName("Import Result")]
        public List<ItemPrice> ItemPrices { get; set; }
    }
}
