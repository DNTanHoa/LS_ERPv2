using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class LoadingPlanImportParam
    {
        public Customer Customer { get; set; }
        [XafDisplayName("File Path")]
        [ImmediatePostData(true)]
        public string ImportFilePath { get; set; }
        public List<LoadingPlan> Data { get; set; } = new List<LoadingPlan>();
    }
}
