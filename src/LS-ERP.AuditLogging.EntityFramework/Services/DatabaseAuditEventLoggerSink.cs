using LS_ERP.AuditLogging.EntityFramework.Entities;
using LS_ERP.AuditLogging.EntityFramework.Mapping;
using LS_ERP.AuditLogging.EntityFramework.Repositories;
using LS_ERP.AuditLogging.Events;
using LS_ERP.AuditLogging.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.EntityFramework.Services
{
    public class DatabaseAuditEventLoggerSink<TAuditLog> : IAuditEventLoggerSink
        where TAuditLog : AuditLog, new()
    {
        private readonly IAuditLoggingRepository<TAuditLog> _auditLoggingRepository;

        public DatabaseAuditEventLoggerSink(IAuditLoggingRepository<TAuditLog> auditLoggingRepository)
        {
            _auditLoggingRepository = auditLoggingRepository;
        }

        public virtual async Task PersistAsync(AuditEvent auditEvent)
        {
            if (auditEvent == null) throw new ArgumentNullException(nameof(auditEvent));

            var auditLog = auditEvent.MapToEntity<TAuditLog>();

            await _auditLoggingRepository.SaveAsync(auditLog);
        }
    }
}
