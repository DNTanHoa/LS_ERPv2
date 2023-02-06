using LS_ERP.AuditLogging.Configuration;
using LS_ERP.AuditLogging.Helpers.HttpContextHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Events.Http
{
    public class HttpAuditAction : IAuditAction
    {
        public HttpAuditAction(IHttpContextAccessor accessor, AuditHttpActionOptions options)
        {
            Action = new
            {
                TraceIdentifier = accessor.HttpContext.TraceIdentifier,
                RequestUrl = accessor.HttpContext.Request.GetDisplayUrl(),
                HttpMethod = accessor.HttpContext.Request.Method,
                FormVariables = options.IncludeFormVariables ? HttpContextHelpers.GetFormVariables(accessor.HttpContext) : null
            };
        }

        public object Action { get; set; }
    }
}
