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
    public class ImportStorageCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<List<ImportStorageDto>>>
    {
        public string CustomerID { get; set; }
        public string CompanyID { get; set; }
        public string StorageCode { get; set; }
        public bool Output { get; set; }
        public IFormFile File { get; set; }
    }
}
