using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class ImportStorageDetailResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<ImportStorageDto> Data { get; set; } = new List<ImportStorageDto>();
    }
}
