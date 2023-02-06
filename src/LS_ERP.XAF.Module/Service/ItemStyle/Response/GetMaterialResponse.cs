using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class GetMaterialResponse : CommonRespone
    {
        public List<ItemStyleMaterial> Data { get; set; }
            = new List<ItemStyleMaterial>();
    }
}
