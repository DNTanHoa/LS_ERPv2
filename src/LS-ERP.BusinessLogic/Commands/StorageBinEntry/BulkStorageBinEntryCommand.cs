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
    public class BulkStorageBinEntryCommand : CommonAuditCommand,
        IRequest<CommonCommandResult>
    {
        public string StorageCode { get; set; }
        public string CustomerID { get; set; }
        public string Factory { get; set; }
        public List<ImportStorageBinEntryDto> Data { get; set; }
            = new List<ImportStorageBinEntryDto>();
    }
}
