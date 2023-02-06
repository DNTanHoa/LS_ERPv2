using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ViewProductionBomParam
    {
        public ItemStyle ItemStyle { get; set; }

        public List<ProductionBOM> ProductionBOMs { get; set; }
    }
}
