using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Dtos.SalesOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands.Result
{
    public class CompareDataResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public GroupCompareDto GroupCompare { get; set; }
    }

}
