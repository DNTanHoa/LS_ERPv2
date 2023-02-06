using Common.Model;
using LS_ERP.BusinessLogic.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class ImportStorageBinEntryCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<List<ImportStorageBinEntryDto>>>
    {
        public string StorageCode { get; set; } = string.Empty;
        public string CustomerID { get; set; }
        public IFormFile File { get; set; }
    }
}
