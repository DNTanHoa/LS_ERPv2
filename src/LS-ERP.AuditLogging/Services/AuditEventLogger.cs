using LS_ERP.AuditLogging.Configuration;
using LS_ERP.AuditLogging.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Services
{
    public class AuditEventLogger : IAuditEventLogger
    {
        protected readonly IEnumerable<IAuditEventLoggerSink> Sinks;
        protected readonly IAuditSubject AuditSubject;
        protected readonly IAuditAction AuditAction;
        private readonly AuditLoggerOptions _auditLoggerOptions;

        public AuditEventLogger(IEnumerable<IAuditEventLoggerSink> sinks, IAuditSubject auditSubject, IAuditAction auditAction, AuditLoggerOptions auditLoggerOptions)
        {
            Sinks = sinks;
            AuditSubject = auditSubject;
            AuditAction = auditAction;
            _auditLoggerOptions = auditLoggerOptions;
        }

        /// <summary>
        /// Prepare default values for an event
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <param name="loggerOptions"></param>
        /// <returns></returns>
        protected virtual Task PrepareEventAsync(AuditEvent auditEvent, Action<AuditLoggerOptions> loggerOptions)
        {
            if (loggerOptions == default)
            {
                PrepareDefaultValues(auditEvent, _auditLoggerOptions);
            }
            else
            {
                var auditLoggerOptions = new AuditLoggerOptions();
                loggerOptions.Invoke(auditLoggerOptions);
                PrepareDefaultValues(auditEvent, auditLoggerOptions);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Prepare default values according to logger options
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <param name="loggerOptions"></param>
        private void PrepareDefaultValues(AuditEvent auditEvent, AuditLoggerOptions loggerOptions)
        {
            if (loggerOptions.UseDefaultSubject)
            {
                PrepareDefaultSubject(auditEvent);
            }

            if (loggerOptions.UseDefaultAction)
            {
                PrepareDefaultAction(auditEvent);
            }

            PrepareDefaultConfiguration(auditEvent, loggerOptions);
        }

        /// <summary>
        /// Prepare default configuration
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <param name="loggerOptions"></param>
        private void PrepareDefaultConfiguration(AuditEvent auditEvent, AuditLoggerOptions loggerOptions)
        {
            auditEvent.Source = loggerOptions.Source;
        }

        /// <summary>
        /// Prepare default action from IAuditAction
        /// </summary>
        /// <param name="auditEvent"></param>
        private void PrepareDefaultAction(AuditEvent auditEvent)
        {
            auditEvent.Action = AuditAction.Action;
        }

        /// <summary>
        /// Prepare default subject from IAuditSubject
        /// </summary>
        /// <param name="auditEvent"></param>
        private void PrepareDefaultSubject(AuditEvent auditEvent)
        {
            auditEvent.SubjectName = AuditSubject.SubjectName;
            auditEvent.SubjectIdentifier = AuditSubject.SubjectIdentifier;
            auditEvent.SubjectType = AuditSubject.SubjectType;
            auditEvent.SubjectAdditionalData = AuditSubject.SubjectAdditionalData;
        }

        /// <summary>
        /// Log an event
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <param name="loggerOptions"></param>
        /// <returns></returns>
        public virtual async Task LogEventAsync(AuditEvent auditEvent, Action<AuditLoggerOptions> loggerOptions = default)
        {
            await PrepareEventAsync(auditEvent, loggerOptions);

            foreach (var sink in Sinks)
            {
                await sink.PersistAsync(auditEvent);
            }
        }
    }
}
