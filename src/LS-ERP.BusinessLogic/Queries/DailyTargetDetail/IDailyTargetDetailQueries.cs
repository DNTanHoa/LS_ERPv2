using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.XAF.Module.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IDailyTargetDetailQueries
    {
        public IEnumerable<DailyTargetDetailDtos> GetAll();
        public IEnumerable<DailyTargetDetailDtos> GetByDate(DateTime date,List<string> departmentIds);
        public IEnumerable<DailyTargetDetailDtos> GetByDate(DateTime date);
        public IEnumerable<MonthDailyTargetDetailDtos> GetByMonth(string companyID, DateTime date);
        public IEnumerable<DailyTargetDetailDtos> GetToDate(string companyID,DateTime fromDate, DateTime toDate);
        public IEnumerable<DailyTargetDetailDtos> GetById(int id);
        public IEnumerable<DailyTargetDetailDtos> GetByWorkCenterId(string departmentId, DateTime produceDate);
        public IEnumerable<DailyTargetDetailDtos> GetByOperation(string companyId, DateTime fromDate, DateTime toDate, string operation);
        public IEnumerable<DailyTargetDetailDtos> GetByOperation(string companyId, DateTime produceDate, string operation);
        public IEnumerable<AllocDailyOutputDtos> GetAllocBySize(string companyId, DateTime fromDate, DateTime toDate, string operation);
        public IEnumerable<DailyTargetDetailDtos> GetByOperationDate(string customerId, string operation, DateTime produceDate);

        public IEnumerable<XAFDailyTargetDetailSummaryDtos> GetDailyTargetDetailSummary (string customerID, string style, DateTime fromDate, DateTime toDate);

        public IEnumerable<DailyTargetDetailSummaryDtos> GetSummaryByMonth(string companyId, DateTime fromDate, DateTime toDate);

        public IEnumerable<LSStyleOrderQuantityDtos> GetOrderOutputQuantity(List<string> LSStyles);
        public IEnumerable<DailyTargetDetailDtos> GetByOffset(string companyId, DateTime produceDate, string operation);
    }
}
