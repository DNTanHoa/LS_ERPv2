using LS_ERP.XAF.Module.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class GroupToIssuedLineRequest
    {
        public string IssuedNumber { get; set; }
        public List<ProductionBOMDto> ProductionBOMs { get; set; }
        public List<IssuedLineDto> IssuedLines { get; set; }
        public List<IssuedGroupLineDto> IssuedGroupLines { get; set; }
    }
}
