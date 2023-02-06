using DevExpress.ExpressApp.Data;
using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class UpdateStatusParam
    {
        public StorageStatus Status { get; set; }
        public string Reason { get; set; }
        public List<StorageDetailsReport> Details { get; set; } = new List<StorageDetailsReport>();
    }
}
