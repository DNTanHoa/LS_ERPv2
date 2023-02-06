using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IAllocDailyOutputQueries
    {
        public IEnumerable<AllocDailyOutputDtos> GetByTargetID(int ID);
        public IEnumerable<AllocDailyOutputDtos> GetByID(int ID);

    }
}
