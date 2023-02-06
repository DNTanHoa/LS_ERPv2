using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories
{
    public class FabricRequestDetailLogRepository : IFabricRequestDetailLogRepository
    {
        private readonly AppDbContext context;

        public FabricRequestDetailLogRepository(AppDbContext context)
        {
            this.context = context;
        }
        public FabricRequestDetailLog Add(FabricRequestDetailLog fabricRequestDetailLog)
        {
            throw new NotImplementedException();
        }

        public void Delete(FabricRequestDetailLog fabricRequestDetailLog)
        {
            throw new NotImplementedException();
        }

        public FabricRequestDetailLog GetFabricRequestDetailLog(long? ID)
        {
            throw new NotImplementedException();
        }

        public IQueryable<FabricRequestDetailLog> GetFabricRequestDetailLogs()
        {
            throw new NotImplementedException();
        }

        public IQueryable<FabricRequestDetailLog> GetFabricRequestDetailLogs(long fabricRequestLogID)
        {
            return context.FabricRequestDetailLog.Where(x => x.FabricRequestLogID == fabricRequestLogID);
        }

        public void Update(FabricRequestDetailLog fabricRequestDetailLog)
        {
            throw new NotImplementedException();
        }
    }
}
