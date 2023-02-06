using DevExpress.XtraRichEdit.Import.Doc;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Report
{
    public class MaterialWithSizeBreakDownModel : MaterialRequest
    {
        public string CustomerStyle { get; set; }

        public List<SizeBreakDownModel> Sizes { get; set; } = new List<SizeBreakDownModel>();
    }
}
