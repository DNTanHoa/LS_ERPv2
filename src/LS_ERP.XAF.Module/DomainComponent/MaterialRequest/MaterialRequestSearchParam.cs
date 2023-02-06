using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class MaterialRequestSearchParam
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Search { get; set; }
        public Storage StorageCode { get; set; }
        public List<MaterialRequest> MaterialRequests { get; set; }
        = new List<MaterialRequest>();
    }
}
