using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories
{
    public class FabricRequestLogRepository : IFabricRequestLogRepository
    {
        private readonly AppDbContext context;

        public FabricRequestLogRepository(AppDbContext context)
        {
            this.context = context;
        }

        public FabricRequestLog Add(FabricRequestLog fabricRequestLog)
        {
            throw new NotImplementedException();
        }

        public void Delete(FabricRequestLog fabricRequestLog)
        {
            throw new NotImplementedException();
        }

        public FabricRequestLog GetFabricRequestLog(long? ID)
        {
            throw new NotImplementedException();
        }

        public IQueryable<FabricRequestLog> GetFabricRequestLogs()
        {
            throw new NotImplementedException();
        }

        public IQueryable<FabricRequestLog> GetFabricRequestLogs(string CustomerID)
        {
            throw new NotImplementedException();
        }

        public IQueryable<FabricRequestLog> GetFabricRequestLogs(string CustomerID, string CompanyCode, DateTime? fromDate, DateTime? toDate)
        {
            throw new NotImplementedException();
        }

        public IQueryable<FabricRequestLog> GetFabricRequestLogs(long? ID)
        {
            return context.FabricRequestLog
                            .Include(x => x.Details)
                            .Include(x => x.Status)
                            .Where(x => x.FabricRequestID == ID);
        }

        public FabricRequestLog GetOnlyFabricRequestLog(long? ID)
        {
            throw new NotImplementedException();
        }

        public void Update(FabricRequestLog fabricRequestLog)
        {
            throw new NotImplementedException();
        }
    }
}
