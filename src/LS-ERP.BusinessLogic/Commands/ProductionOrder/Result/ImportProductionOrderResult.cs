﻿using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class ImportProductionOrderResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<ProductionOrderLine> Result { get; set; }
    }

    public class ProductionOrderLineImportData : ProductionOrderLine
    {
        public bool IsHasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
