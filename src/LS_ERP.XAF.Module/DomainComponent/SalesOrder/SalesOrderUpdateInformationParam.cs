using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class SalesOrderUpdateInformationParam
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public string FilePath { get; set; }
        public List<SalesOrderUpdateInformation> Data { get; set; }
    }

    [DomainComponent]
    public class SalesOrderUpdateInformation
    {
        public string LSStyle { get; set; }
        public DateTime? IssuedDate { get; set; }
        public DateTime? ProductionSketDeliveryDate { get; set; }
        public DateTime? AccessoriesDate { get; set; }
        public DateTime? FabricDate { get; set; }
        public string Remark { get; set; }
    }
}
