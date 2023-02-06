﻿using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.XAF.Module.Dtos.SalesOrder;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class SaveCompareCommand : CommonAuditCommand,
         IRequest<CompareDataResult>
    {
        public string CustomerID { get; set; }
        public string BrandCode { get; set; }
        public string CustomerStyle { get; set; }
        public DateTime? FromDate { get; set; }
        public bool? IsSaveCompare { get; set; }
        public GroupCompareDto GroupCompare { get; set; } = new GroupCompareDto();
    }
}
