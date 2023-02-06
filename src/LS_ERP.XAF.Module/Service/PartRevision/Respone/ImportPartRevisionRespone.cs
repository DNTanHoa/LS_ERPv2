using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Respone
{
    public class ImportPartRevisionRespone : CommonRespone
    {
        public GroupToPartRevisionResponeData Data { get; set; }

        public new ImportPartRevisionRespone SetResult(bool IsSuccess, string Message)
        {
            this.Result = new CommonResult(IsSuccess, Message);
            return this;
        }
    }

    public class GroupToPartRevisionResponeData
    {
        public PartRevision PartRevision { get; set; }
        public List<Item> Items { get; set; }
        public string FileName { get; set; }
        public string FileNameServer { get; set; }
    }
}
