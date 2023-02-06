using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Response
{
    public class GetJobOperationResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<JobOperation> Data { get; set; }
    }
}
