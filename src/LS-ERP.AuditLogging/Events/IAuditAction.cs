using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Events
{
    public interface IAuditAction
    {
        object Action { get; set; }
    }
}
