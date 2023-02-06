using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class JobOperationSearchParam
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public string Style { get; set; }
        public Customer Customer { get; set; }
        public Operation Operation { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<JobHead> JobHeads { get; set; } = new List<JobHead>();
        public List<JobOperation> JobOperations { get; set; } = new List<JobOperation>();
    }
}
