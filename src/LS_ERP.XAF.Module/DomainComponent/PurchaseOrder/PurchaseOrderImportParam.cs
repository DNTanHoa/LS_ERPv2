using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class PurchaseOrderImportParam
    {
        [ImmediatePostData(true)]
        public Customer Customer { get; set; }
        [DataSourceProperty("Customer.Brands")]
        public Brand Brand { get; set; }
        [XafDisplayName("File")]
        public string ImportFilePath { get; set; }
        public TypeImportPurchaseOrder Type { get; set; }

    }

    public enum TypeImportPurchaseOrder
    {
        [XafDisplayName("Fabric Tracking")]
        FabricTracking,
        [XafDisplayName("Trim Tracking")]
        TrimTracking,
        [XafDisplayName("Purchase detail + OCL")]
        PurchaseDetail,
        [XafDisplayName("Care label")]
        Carelabel
    }
}
