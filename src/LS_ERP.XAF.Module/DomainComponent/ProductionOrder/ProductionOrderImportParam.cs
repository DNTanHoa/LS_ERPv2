using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ProductionOrderImportParam
    {
        public string FilePath { get; set; } = string.Empty;
        public Customer Customer { get; set; }
        public DateTime StartDate { get; set; }

        public List<ProductionOrderLine> Lines { get; set; }
            = new List<ProductionOrderLine>();
    }
}
