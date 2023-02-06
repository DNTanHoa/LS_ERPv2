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
    public class FabricRequestRepository : IFabricRequestRepository
    {
        private readonly AppDbContext context;

        public FabricRequestRepository(AppDbContext context)
        {
            this.context = context;
        }

        public FabricRequest Add(FabricRequest fabricRequest)
        {
            return context.Add(fabricRequest).Entity;
        }

        public void Delete(FabricRequest fabricRequest)
        {
            context.FabricRequest.Remove(fabricRequest);
        }

        public FabricRequest GetFabricRequest(long? ID)
        {
            return context.FabricRequest
                          .Include(x => x.Details)
                          .Include(x => x.Status)
                          .FirstOrDefault(x => x.ID == ID);
        }

        public FabricRequest GetOnlyFabricRequest(long? ID)
        {
            return context.FabricRequest.FirstOrDefault(x => x.ID == ID);
        }

        public IQueryable<FabricRequest> GetFabricRequests()
        {
            return context.FabricRequest;
        }

        public IQueryable<FabricRequest> GetFabricRequests(string CustomerID)
        {
            return context.FabricRequest;
        }

        public IQueryable<FabricRequest> GetFabricRequests(string CustomerID, string CompanyCode, DateTime? fromDate, DateTime? toDate)
        {
            if (!string.IsNullOrEmpty(CustomerID) && !string.IsNullOrEmpty(CompanyCode) && fromDate != null && toDate != null)
            {
                return context.FabricRequest
                          .Include(x => x.Details)
                          .Include(x => x.Status)
                          .Where(x => x.CustomerID == CustomerID && x.CompanyCode == CompanyCode &&
                          DateTime.Compare(x.CreatedAt ?? DateTime.Now, fromDate ?? DateTime.Now) >= 0 &&
                           DateTime.Compare(x.CreatedAt ?? DateTime.Now, toDate.Value.AddDays(1)) < 0)
                          .OrderByDescending(x => x.CreatedAt);
            }
            else if (!string.IsNullOrEmpty(CustomerID) && !string.IsNullOrEmpty(CompanyCode))
            {
                return context.FabricRequest
                          .Include(x => x.Details)
                          .Include(x => x.Status).Where(x => x.CustomerID == CustomerID && x.CompanyCode == CompanyCode)
                          .OrderByDescending(x => x.CreatedAt);
            }
            else if (!string.IsNullOrEmpty(CustomerID) && fromDate != null && toDate != null)
            {
                return context.FabricRequest
                          .Include(x => x.Details)
                          .Include(x => x.Status).Where(x => x.CustomerID == CustomerID &&
                           DateTime.Compare(x.CreatedAt ?? DateTime.Now, fromDate ?? DateTime.Now) >= 0 &&
                           DateTime.Compare(x.CreatedAt ?? DateTime.Now, toDate.Value.AddDays(1)) < 0)
                          .OrderByDescending(x => x.CreatedAt);
            }
            else if (!string.IsNullOrEmpty(CompanyCode) && fromDate != null && toDate != null)
            {
                return context.FabricRequest
                          .Include(x => x.Details)
                          .Include(x => x.Status).Where(x => x.CompanyCode == CompanyCode &&
                           DateTime.Compare(x.CreatedAt ?? DateTime.Now, fromDate ?? DateTime.Now) >= 0 &&
                           DateTime.Compare(x.CreatedAt ?? DateTime.Now, toDate.Value.AddDays(1)) < 0)
                          .OrderByDescending(x => x.CreatedAt);
            }
            else if (!string.IsNullOrEmpty(CustomerID))
            {
                return context.FabricRequest
                          .Include(x => x.Details)
                          .Include(x => x.Status).Where(x => x.CustomerID == CustomerID)
                          .OrderByDescending(x => x.CreatedAt);
            }
            else if (!string.IsNullOrEmpty(CompanyCode))
            {
                return context.FabricRequest
                          .Include(x => x.Details)
                          .Include(x => x.Status).Where(x => x.CompanyCode == CompanyCode)
                          .OrderByDescending(x => x.CreatedAt);
            }
            else
            {
                return context.FabricRequest
                          .Include(x => x.Details)
                          .Include(x => x.Status)
                          .OrderByDescending(x => x.CreatedAt);
            }
        }

        public void Update(FabricRequest fabricRequest)
        {
            context.Entry(fabricRequest).State = EntityState.Modified;
        }
    }
}
