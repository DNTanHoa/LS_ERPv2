using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Dtos.Invoice
{
    public class DocumentFileInfoDto
    {
        public string FileName { get; set; }
        public string FileNameServer { get; set; }
        public string FilePath { get; set; }
        public int InvoiceDocumentTypeID { get; set; }
    }
}
