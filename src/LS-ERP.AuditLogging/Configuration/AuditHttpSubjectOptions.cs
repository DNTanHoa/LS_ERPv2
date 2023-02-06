using LS_ERP.AuditLogging.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Configuration
{
    public class AuditHttpSubjectOptions
    {
        public string SubjectIdentifierClaim { get; set; } = ClaimsConsts.Sub;

        public string SubjectNameClaim { get; set; } = ClaimsConsts.Name;
    }
}
