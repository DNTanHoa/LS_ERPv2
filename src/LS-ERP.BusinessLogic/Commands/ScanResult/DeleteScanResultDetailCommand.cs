using Common.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class DeleteScanResultDetailCommand
        :IRequest<CommonCommandResult>
    {
        public List<long> Data { get; set; } = new List<long>();
        public string UserName { get; set; }    
    }
}
