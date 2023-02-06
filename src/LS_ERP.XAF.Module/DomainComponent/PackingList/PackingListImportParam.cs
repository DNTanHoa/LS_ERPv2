using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class PackingListImportParam
    {
        [ImmediatePostData(true)]
        public Customer Customer { get; set; }

        [XafDisplayName("File")]
        [ImmediatePostData(true)]
        public string ImportFilePath { get; set; }
        [Browsable(false)]
        public List<PackingList> Data { get; set; } = new List<PackingList>();
    }
}
