﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class ImportPackingListRequest
    {
        public string CustomerID { get; set; }
        public string FilePath { get; set; }
        public string UserName { get; set; }
    }
}