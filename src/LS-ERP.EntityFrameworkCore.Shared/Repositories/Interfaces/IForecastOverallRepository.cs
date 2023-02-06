using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IForecastOverallRepository
    {
        ForecastOverall Add(ForecastOverall ForecastOverall);
        void Update(ForecastOverall ForecastOverall);
        void Delete(ForecastOverall ForecastOverall);
        IQueryable<ForecastOverall> GetForecastOveralls();
        IQueryable<ForecastOverall> GetForecastOveralls(List<string> IDs);
        ForecastOverall GetForecastOverall(string ID);
        bool IsExist(string ID);
        bool IsExist(string ID, out ForecastOverall ForecastOverall);
    }
}
