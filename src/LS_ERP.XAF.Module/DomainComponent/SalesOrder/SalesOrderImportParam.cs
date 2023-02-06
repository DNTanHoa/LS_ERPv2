using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Dtos.SalesOrder;
using System;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SalesOrderImportParam
    {
        [ImmediatePostData(true)]
        public Customer Customer { get; set; }
        [DataSourceProperty("Customer.Brands")]
        public Brand Brand { get; set; }
        [XafDisplayName("Style")]
        public string Style { get; set; }
        [XafDisplayName("File")]
        [ImmediatePostData(true)]
        public string ImportFilePath { get; set; }
        public DateTime? ConfirmDate { get; set; }
        [XafDisplayName("Update")]
        public bool? IsUpdate { get; set; }
        [Browsable(false)]
        public GroupCompareDto GroupCompare { get; set; }
    }
}
