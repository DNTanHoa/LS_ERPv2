using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.BusinessLogic.Commands.Result
{
    public class ImportPackingListResult
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public List<PackingList> Data { get; set; }
    }
}
