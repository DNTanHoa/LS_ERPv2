using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IWorkCenterQueries
    {
        public IEnumerable<WorkCenterDtos> Get(string departmentId);
        public IEnumerable<WorkCenterDtos> Get();
        public IEnumerable<WorkCenterDtos> Get(List<string> listWorkCenterID);
        public IEnumerable<WorkCenterDtos> GetCuttingCenterByCompany(string companyID);
        public IEnumerable<WorkCenterDtos> GetSewingCenterByCompany(string companyID);
    }
}
