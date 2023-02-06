using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IFabricRequestDetailRepository
    {
        FabricRequestDetail Add(FabricRequestDetail fabricRequestDetail);
        void Update(FabricRequestDetail fabricRequestDetail);
        void Delete(FabricRequestDetail fabricRequestDetail);
        IQueryable<FabricRequestDetail> GetFabricRequestDetails();
        IQueryable<FabricRequestDetail> GetFabricRequestDetails(long fabricRequestID);
        FabricRequestDetail GetFabricRequestDetail(long? ID);
    }
}
