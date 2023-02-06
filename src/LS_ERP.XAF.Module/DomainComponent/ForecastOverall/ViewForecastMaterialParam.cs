using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ViewForecastMaterialParam
    {
        public ForecastOverall ForecastOverall { get; set; }
        public List<ForecastMaterial> ForecastMaterials { get; set; }
    }
}
