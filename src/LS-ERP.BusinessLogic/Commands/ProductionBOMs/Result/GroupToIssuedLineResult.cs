using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class GroupToIssuedLineResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<IssuedGroupLine> IssuedGroupLines { get; set; }
        public List<IssuedLine> IssuedLines { get; set; }
    }
}
