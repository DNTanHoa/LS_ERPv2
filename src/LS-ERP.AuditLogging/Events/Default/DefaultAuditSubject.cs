using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Events.Default
{
    public class DefaultAuditSubject : IAuditSubject
    {
        public string SubjectIdentifier { get; set; }
        public string SubjectName { get; set; }
        public string SubjectType { get; set; }
        public object SubjectAdditionalData { get; set; }
    }
}
