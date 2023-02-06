using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service
{
    public class ImportPartRevisionRequest
    {
        public string UserName { get; set; }
        public string StyleNumber { get; set; }
        public string RevisionNumber { get; set; }
        public string CustomerID { get; set; }
        public string Season { get; set; }
        public DateTime? EffectDate { get; set; }
        public bool? IsConfirmed { get; set; }
        public string FilePath { get; set; }

        public string FileName { get; set; }
        public string FileNameServer { get; set; }
    }
}
