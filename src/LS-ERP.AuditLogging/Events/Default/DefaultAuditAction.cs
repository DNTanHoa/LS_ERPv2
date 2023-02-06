using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Events.Default
{
    public class DefaultAuditAction : IAuditAction
    {
        public object Action { get; set; }
    }
}
