﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class CreateProductionOrderResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string ProductionID { get; set; }
    }
}
