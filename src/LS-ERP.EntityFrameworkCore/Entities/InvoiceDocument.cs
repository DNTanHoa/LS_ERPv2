using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class InvoiceDocument : Audit
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string FileNameServer { get; set; }
        public string FilePath { get; set; }
        public int InvoiceDocumentTypeID { get; set; }
        public virtual InvoiceDocumentType InvoiceDocumentType { get; set; }
        public virtual Invoice Invoice { get; set; }
    }
}
