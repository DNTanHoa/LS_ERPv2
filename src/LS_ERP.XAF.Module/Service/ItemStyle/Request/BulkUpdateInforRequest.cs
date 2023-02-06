using LS_ERP.XAF.Module.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Request
{
    public class BulkUpdateInforRequest
    {
        public string UserName { get; set; }
        public List<ItemStyleInfoDto> Data { get; set; }
    }
}
