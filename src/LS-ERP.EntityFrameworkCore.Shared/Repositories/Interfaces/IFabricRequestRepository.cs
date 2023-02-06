using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IFabricRequestRepository
    {
        FabricRequest Add(FabricRequest fabricRequest);
        void Update(FabricRequest fabricRequest);
        void Delete(FabricRequest fabricRequest);
        IQueryable<FabricRequest> GetFabricRequests();
        IQueryable<FabricRequest> GetFabricRequests(string CustomerID);
        IQueryable<FabricRequest> GetFabricRequests(string CustomerID, string CompanyCode, DateTime? fromDate, DateTime? toDate);
        FabricRequest GetFabricRequest(long? ID);
        FabricRequest GetOnlyFabricRequest(long? ID);
    }
}
