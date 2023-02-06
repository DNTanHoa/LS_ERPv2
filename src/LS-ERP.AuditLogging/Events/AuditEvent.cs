using LS_ERP.AuditLogging.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Events
{
    public abstract class AuditEvent
    {
        protected AuditEvent()
        {
            Event = GetType().GetNameWithoutGenericParams();
        }

        public string Event { get; set; }
        public string Source { get; set; }
        public string Category { get; set; }
        public string SubjectIdentifier { get; set; }
        public string SubjectName { get; set; }
        public string SubjectType { get; set; }
        public object SubjectAdditionalData { get; set; }
        public object Action { get; set; }
    }
}
