using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ForecastGroupImportParam
    {
        public ForecastGroup ForecastGroup { get; set; }
        public Customer Customer { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }

        public List<ForecastOverall> Overalls { get; set; }
    }
}
