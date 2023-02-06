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
    public class ForecastOverallRepository : IForecastOverallRepository
    {
        private readonly AppDbContext context;

        public ForecastOverallRepository(AppDbContext context)
        {
            this.context = context;
        }

        public ForecastOverall Add(ForecastOverall ForecastOverall)
        {
            return context.ForecastOverall.Add(ForecastOverall).Entity;
        }

        public void Delete(ForecastOverall ForecastOverall)
        {
            context.ForecastOverall.Remove(ForecastOverall);
        }

        public ForecastOverall GetForecastOverall(string ID)
        {
            return context.ForecastOverall.FirstOrDefault(x => x.ID == ID);
        }

        public IQueryable<ForecastOverall> GetForecastOveralls()
        {
            return context.ForecastOverall;
        }

        public IQueryable<ForecastOverall> GetForecastOveralls(List<string> IDs)
        {
            return context.ForecastOverall
                          .Include(x => x.ForecastDetails)
                          .ThenInclude(x => x.ForecastMaterials)
                          .Where(x => IDs.Contains(x.ID));
        }

        public bool IsExist(string ID)
        {
            var forecastOverall = GetForecastOverall(ID);
            return forecastOverall != null ? true : false;
        }

        public bool IsExist(string ID, out ForecastOverall forecastOverall)
        {
            forecastOverall = null;
            forecastOverall = GetForecastOverall(ID);
            return forecastOverall != null ? true : false;
        }

        public void Update(ForecastOverall ForecastOverall)
        {
            context.Entry(ForecastOverall).State = EntityState.Modified;
        }
    }
}
