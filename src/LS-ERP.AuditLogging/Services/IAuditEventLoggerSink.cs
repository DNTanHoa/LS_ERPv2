using LS_ERP.AuditLogging.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Services
{
    public interface IAuditEventLoggerSink
    {
        Task PersistAsync(AuditEvent auditEvent);
    }
}
