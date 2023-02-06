using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Report
{
    public class MaterialRequestDetailModel : MaterialRequestDetail
    {
        public MaterialRequestReportModel MaterialRequest { get; set; }
        public string ItemDescription { get; set; } = string.Empty;
    }
}
