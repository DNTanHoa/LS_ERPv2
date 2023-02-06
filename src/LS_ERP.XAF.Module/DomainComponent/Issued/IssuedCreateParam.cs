using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class IssuedCreateParam
    {
        public string StyleNumher { get; set; }
        public string ContractNo { get; set; }
        public string CustomerID { get; set; }
        public string StorageCode { get; set; }
        public string Description { get; set; }
        public List<ItemStyle> Styless { get; set; }
        public List<ProductionBOM> ProductionBOMs { get; set; }
    }
}
