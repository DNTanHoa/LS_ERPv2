using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IFabricRequestLogRepository
    {
        FabricRequestLog Add(FabricRequestLog fabricRequestLog);
        void Update(FabricRequestLog fabricRequestLog);
        void Delete(FabricRequestLog fabricRequestLog);
        IQueryable<FabricRequestLog> GetFabricRequestLogs();
        IQueryable<FabricRequestLog> GetFabricRequestLogs(long? ID);
        IQueryable<FabricRequestLog> GetFabricRequestLogs(string CustomerID);
        IQueryable<FabricRequestLog> GetFabricRequestLogs(string CustomerID, string CompanyCode, DateTime? fromDate, DateTime? toDate);
        FabricRequestLog GetFabricRequestLog(long? ID);
        FabricRequestLog GetOnlyFabricRequestLog(long? ID);
    }
}
