using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands.Result
{
    public class GetIssuedSupplierInfoResult
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public IssuedSupplierDto Result { get; set; }
    }
}
