using LS_ERP.BusinessLogic.Commands.Result;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class CancelOrderDetailCommand : IRequest<CancelOrderDetailResult>
    {
        public List<long> OrderDetailIDs { get; set; }
    }
}
