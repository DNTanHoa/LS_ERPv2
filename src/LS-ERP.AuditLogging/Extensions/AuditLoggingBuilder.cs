using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.Extensions
{
    public class AuditLoggingBuilder : IAuditLoggingBuilder
    {
        public AuditLoggingBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
