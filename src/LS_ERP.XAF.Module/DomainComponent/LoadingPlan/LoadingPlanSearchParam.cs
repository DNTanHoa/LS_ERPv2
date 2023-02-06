using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    [XafDisplayName("Loading Plan")]
    public class LoadingPlanSearchParam
    {
        [Browsable(false)]
        [DevExpress.ExpressApp.Data.Key]
        public int ID { get; set; }
        public Customer Customer { get; set; }
        public string OrderNumber { get; set; }
        public string ContainerNumber { get; set; }
        public List<LoadingPlan> LoadingPlans { get; set; } = new List<LoadingPlan>();
    }
}
