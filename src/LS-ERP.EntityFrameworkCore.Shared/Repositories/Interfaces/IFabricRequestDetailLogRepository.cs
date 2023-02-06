using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IFabricRequestDetailLogRepository
    {
        FabricRequestDetailLog Add(FabricRequestDetailLog fabricRequestDetailLog);
        void Update(FabricRequestDetailLog fabricRequestDetailLog);
        void Delete(FabricRequestDetailLog fabricRequestDetailLog);
        IQueryable<FabricRequestDetailLog> GetFabricRequestDetailLogs();
        IQueryable<FabricRequestDetailLog> GetFabricRequestDetailLogs(long fabricRequestID);
        FabricRequestDetailLog GetFabricRequestDetailLog(long? ID);
    }
}
