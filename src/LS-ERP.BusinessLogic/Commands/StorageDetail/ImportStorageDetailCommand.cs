using LS_ERP.BusinessLogic.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LS_ERP.BusinessLogic.Commands
{
    public class ImportStorageDetailCommand : CommonAuditCommand,
        IRequest<ImportStorageDetailResult>
    {
        public string StorageCode { get; set; }
        public string CustomerID { get; set; }
        public IFormFile File { get; set; }
    }
}
