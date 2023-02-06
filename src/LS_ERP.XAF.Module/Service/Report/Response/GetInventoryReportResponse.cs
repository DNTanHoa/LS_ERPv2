﻿using LS_ERP.XAF.Module.DomainComponent.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Report.Response
{
    public class GetInventoryReportResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<InventoryReportDetail> Data { get; set; }
    }

    
}
