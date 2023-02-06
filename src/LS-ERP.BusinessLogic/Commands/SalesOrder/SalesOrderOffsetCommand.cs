using Common.Model;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class SalesOrderOffsetCommand : IRequest<CommonCommandResult>
    {
        public string CustomerID { get; set; }
        public string UserName { get; set; } 
        public List<SalesOrderOffset> Data { get; set; }
        = new List<SalesOrderOffset>();
    }
}
