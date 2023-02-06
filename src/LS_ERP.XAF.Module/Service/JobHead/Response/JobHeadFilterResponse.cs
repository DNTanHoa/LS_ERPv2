using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Response
{
    public class JobHeadFilterResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public List<JobHead> Data { get; set; }
    }
}
