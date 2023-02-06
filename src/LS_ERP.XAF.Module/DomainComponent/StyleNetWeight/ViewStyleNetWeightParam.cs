using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ViewStyleNetWeightParam
    {
        public List<StyleNetWeight> Weights { get; set; }
    }
}
