using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ReturnPopupModel
    {
        public Issued Issued { get; set; }
        public Storage Storage { get; set; }
        public DateTime ReturnDate { get; set; }
        public string ReturnBy { get; set; }
        public string Remark { get; set; }
        public List<ReturnDetailModel> Details { get; set; }
        = new List<ReturnDetailModel>();
    }
}
