using LS_ERP.BusinessLogic.Dtos;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IDailyTargetQueries
    {
        public IEnumerable<DailyTargetDtos> GetAll();
        public IEnumerable<DailyTargetDtos> GetByDate(DateTime date,List<string> departmentIds);
        public IEnumerable<DailyTargetDtos> GetByCompanyID(string companyID, DateTime date);
        public IEnumerable<DailyTargetDtos> GetById(int id);
        public IEnumerable<DailyTargetDtos> GetCuttingTarget(string companyId,DateTime fromDate, DateTime toDate, string operation);
        public IEnumerable<DailyTargetDtos> GetSewingTarget(string companyId, DateTime fromDate, DateTime toDate, string operation);
        public IEnumerable<string> GetMergeBlockLSStyle(string companyId, DateTime fromDate, DateTime toDate,bool isPrint);
        public IEnumerable<string> GetAllMergeBlockLSStyle(string companyId);
        public IEnumerable<string> GetMergeBlockLSStyleQuantity(string companyId, DateTime fromDate, DateTime toDate,
            string mergeBlockLSStyle,string size);
        public IEnumerable<string> GetMergeLSStyleQuantity(string companyId, DateTime fromDate, DateTime toDate, string mergeLSStyle,string size);
        public IEnumerable<string> GetLSStyleQuantity(string companyId, DateTime fromDate, DateTime toDate, string lsStyle,string size);
        public IEnumerable<string> GetCuttingCustomer(string companyId, DateTime fromDate, DateTime toDate);
        public IEnumerable<string> GetCuttingLSStyle(string companyId, DateTime fromDate, DateTime toDate,string mergeLSStyle,string mergeBlockLSStyle,bool isPrint);
        public IEnumerable<string> GetMergeLSStyle(string companyId, DateTime fromDate, DateTime toDate, string mergeBlockLSStyle,bool isPrint);
        public IEnumerable<DailyTargetDtos> GetByWorkCenter(DateTime date, string departmentId);
  
        public IEnumerable<DailyTargetDtos> GetByWorkCenterId(string workCenterId);
        public IEnumerable<DailyTargetDtos> GetByListWorkCenterId(List<string> listWorkCenterId);
        public IEnumerable<LSStyleCompareDtos> CheckOffsetLSStyle(BulkLSStyleCompareDtos bulkLSStyleCompareDtos);
        public IEnumerable<DailyTagetOverviewDtos> GetOverview(string companyID);

    }
}
