﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class ClearingBomRequest
    {
        public string UserName { get; set; }
        public List<string> ItemStyleNumbers { get; set; }
    }
}
