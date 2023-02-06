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
    public class BulkStorageCommand : CommonAuditCommand,
        IRequest<CommonCommandResult>
    {
        public string StorageCode { get; set; }
        public string CustomerID { get; set; }
        public string FileName { get; set; }
        public string ProductionMethodCode { get; set; }
        public bool Output { get; set; }
        public List<ImportStorageDto> Data { get; set; }
            = new List<ImportStorageDto>();
    }
}
