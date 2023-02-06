using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class PullBomParam
    {
        public PullBomType PullBomTypes { get; set; }
    }
}
