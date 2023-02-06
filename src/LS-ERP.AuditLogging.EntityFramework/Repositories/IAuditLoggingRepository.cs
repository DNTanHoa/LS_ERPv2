using LS_ERP.AuditLogging.EntityFramework.Entities;
using LS_ERP.Ultilities.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.AuditLogging.EntityFramework.Repositories
{
    public interface IAuditLoggingRepository<TAuditLog>
    where TAuditLog : AuditLog
    {
        Task SaveAsync(TAuditLog auditLog);

        Task<PagedList<TAuditLog>> GetAsync(int page = 1, int pageSize = 10);

        Task<PagedList<TAuditLog>> GetAsync(string subjectIdentifier, string subjectName, string category, int page = 1,
            int pageSize = 10);
    }
}
