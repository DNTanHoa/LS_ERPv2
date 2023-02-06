using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands.Result
{
    public class GetFabricRequestsResult
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public List<FabricRequest> Result { get; set; }
    }
}
