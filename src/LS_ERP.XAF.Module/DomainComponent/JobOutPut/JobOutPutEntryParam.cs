using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class JobOutPutEntryParam
    {
        [Key]
        [Browsable(false)]
        public int ID { get; set; }
        public string SearchInput { get; set; }
        public Operation Operation { get; set; }
        public DateTime OutputDate { get; set; }
        public List<JobOperation> JobOperations { get; set; }
        public List<JobOutputDetail> Details { get; set; }
    }
}
