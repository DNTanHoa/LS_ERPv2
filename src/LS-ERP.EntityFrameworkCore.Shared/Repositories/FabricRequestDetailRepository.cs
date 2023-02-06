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
    public class FabricRequestDetailRepository : IFabricRequestDetailRepository
    {
        private readonly AppDbContext context;

        public FabricRequestDetailRepository(AppDbContext context)
        {
            this.context = context;
        }

        public FabricRequestDetail Add(FabricRequestDetail fabricRequestDetail)
        {
            return context.Add(fabricRequestDetail).Entity;
        }

        public void Delete(FabricRequestDetail fabricRequestDetail)
        {
            context.FabricRequestDetail.Remove(fabricRequestDetail);
        }

        public FabricRequestDetail GetFabricRequestDetail(long? ID)
        {
            return context.FabricRequestDetail.FirstOrDefault(x => x.ID == ID);
        }

        public IQueryable<FabricRequestDetail> GetFabricRequestDetails()
        {
            return context.FabricRequestDetail;
        }

        public IQueryable<FabricRequestDetail> GetFabricRequestDetails(long fabricRequestID)
        {
            return context.FabricRequestDetail.Where(x => x.FabricRequestID == fabricRequestID);
        }

        public void Update(FabricRequestDetail fabricRequestDetail)
        {
            context.Entry(fabricRequestDetail).State = EntityState.Modified;
        }
    }
}
