using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class StorageBinEntryCreateParam
    {
        public string CustomerPurchaseOrderNumber { get; set; }
        public string CustomerID { get; set; }
        public string CompanyID { get; set; }
        public string StorageCode { get; set; } 
        public Customer Customer { get; set; }
        public Company Factory { get; set; }
        public Storage Storage { get; set; }
        [XafDisplayName("File Path")]
        [ImmediatePostData(true)]
        public string ImportFilePath { get; set; }
        public List<StyleStorageBinEntry> Data { get; set; } = new List<StyleStorageBinEntry>();
    }

    [DomainComponent]
    public class StyleStorageBinEntry
    {
        public string PurchaseOrderNumber { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string Season { get; set; }
        public string BinCode { get; set; }
    }
}
