using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Events
{
    public interface IAuditSubject
    {
        string SubjectIdentifier { get; set; }
        string SubjectName { get; set; }
        string SubjectType { get; set; }
        object SubjectAdditionalData { get; set; }
    }
}
