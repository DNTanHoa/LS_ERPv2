using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Configuration
{
    public class AuditLoggerOptions
    {
        public string Source { get; set; }
        public bool UseDefaultSubject { get; set; } = true;
        public bool UseDefaultAction { get; set; } = true;
    }
}
