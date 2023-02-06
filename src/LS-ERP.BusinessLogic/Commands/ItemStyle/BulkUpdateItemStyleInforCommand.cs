using Common.Model;
using LS_ERP.BusinessLogic.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class BulkUpdateItemStyleInforCommand : CommonAuditCommand,
        IRequest<CommonCommandResult>
    {
        public List<ItemStyleInforDtos> Data { get; set; }
    }
}
