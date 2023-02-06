using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    [XafDisplayName("Quality Assurance")]
    public class QualityAssuranceSearchParam
    {
        [Browsable(false)]
        [DevExpress.ExpressApp.Data.Key]
        public int ID { get; set; }
        public Customer Customer { get; set; }
        public QualityStatus Status { get; set; }
        public decimal? Percent { get; set; }
        public List<QualityAssurance> QualityAssurances { get; set; }
    }
}
