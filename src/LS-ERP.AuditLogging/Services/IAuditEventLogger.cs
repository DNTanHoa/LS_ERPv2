using LS_ERP.AuditLogging.Configuration;
using LS_ERP.AuditLogging.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Services
{
    public interface IAuditEventLogger<out TAuditLoggerOptions>
        where TAuditLoggerOptions : AuditLoggerOptions
    {
        /// <summary>
        /// Log an event
        /// </summary>
        /// <param name="auditEvent">The specific audit event</param>
        /// <param name="loggerOptions"></param>
        /// <returns></returns>
        Task LogEventAsync(AuditEvent auditEvent, Action<TAuditLoggerOptions> loggerOptions = default);
    }

    public interface IAuditEventLogger : IAuditEventLogger<AuditLoggerOptions>
    {

    }
}
