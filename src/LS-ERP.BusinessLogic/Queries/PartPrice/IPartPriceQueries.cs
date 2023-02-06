using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IPartPriceQueries
    {
        public IEnumerable<PartPriceDtos> Get(string companyId, string styleNo);
        public IEnumerable<PartPriceDtos> GetById(int Id);
        public IEnumerable<PartPriceDtos> GetWithDailyTarget(string companyId, string styleNo);
        public IEnumerable<PartPriceDtos> GetWithDailyTarget(string companyId, string styleNo, string item);

    }
}
