using LS_ERP.EntityFrameworkCore.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands.Result
{
    public class ImportSalesContractResult
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public SalesContract Result { get; set; }
    }
}
