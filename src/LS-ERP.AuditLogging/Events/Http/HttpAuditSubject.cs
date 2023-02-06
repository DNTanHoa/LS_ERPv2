using LS_ERP.AuditLogging.Configuration;
using LS_ERP.AuditLogging.Constants;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Events.Http
{
    public class HttpAuditSubject : IAuditSubject
    {
        public HttpAuditSubject(IHttpContextAccessor accessor, AuditHttpSubjectOptions options)
        {
            SubjectIdentifier = accessor.HttpContext.User.FindFirst(options.SubjectIdentifierClaim)?.Value;
            SubjectName = accessor.HttpContext.User.FindFirst(options.SubjectNameClaim)?.Value;
            SubjectAdditionalData = new
            {
                RemoteIpAddress = accessor.HttpContext.Connection?.RemoteIpAddress?.ToString(),
                LocalIpAddress = accessor.HttpContext.Connection?.LocalIpAddress?.ToString(),
                Claims = accessor.HttpContext.User.Claims?.Select(x => new { x.Type, x.Value })
            };
        }

        public string SubjectName { get; set; }

        public string SubjectType { get; set; } = AuditSubjectTypes.User;

        public object SubjectAdditionalData { get; set; }

        public string SubjectIdentifier { get; set; }
    }
}
