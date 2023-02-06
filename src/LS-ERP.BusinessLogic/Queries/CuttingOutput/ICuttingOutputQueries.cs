using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.XAF.Module.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface ICuttingOutputQueries
    {
        public IEnumerable<CuttingOutputDtos> Get(int ID);
        public IEnumerable<CuttingOutputAllocDetailDtos> GetAllocDetail(int ID);
        public IEnumerable<CuttingOutputDtos> Get(string companyID,DateTime fromDate, DateTime toDate);
        public IEnumerable<CuttingOutputDailyReportDtos> GetDailyReport(string companyID, DateTime produceDate);
        public IEnumerable<CuttingOutputDtos> GetMonthReport(string companyID, DateTime produceDate);
        public IEnumerable<CuttingCardDtos> GetCardMergeBlockLSStyle(string companyID, DateTime produceDate);
        public IEnumerable<CuttingOutputDailyReportDtos> GetWorkCenterReportByMonth(string companyID, DateTime produceDate);
        public IEnumerable<PivotCuttingLotDtos> GetPivotCuttingLot(string companyID, DateTime fromDate, DateTime toDate,string mergeBlockLSStyle, string mergeLSStyle);
        public IEnumerable<string> GetIssueCuttingLot(string companyID, DateTime fromDate, DateTime toDate, string mergeBlockLSStyle,int fabricContrastID);
        public IEnumerable<string> GetCuttingSize(string companyID, DateTime fromDate, DateTime toDate, string mergeBlockLSStyle,string mergeLSStyle, string lsStyle);
        public IEnumerable<string> GetCuttingSet(string companyID, DateTime fromDate, DateTime toDate, string mergeBlockLSStyle, string lsStyle);
        public IEnumerable<FabricContrastDtos> GetCuttingFabricContrast(string mergeBlockLSStyle);
        public IEnumerable<CuttingOutputStatusDtos> GetCuttingOutputStatus(string companyID, DateTime fromDate, DateTime toDate,string customerName,string LSStyle);
        public IEnumerable<AllocDailyOutputChangeDtos> CheckChangeOrderQuantity(string companyID);
        public IEnumerable<AllocDailyOutputChangeDtos> UpdateChangeOrderQuantity(List<string> listID);
        public IEnumerable<CuttingOutputReportDtos> GetCuttingOutputReport(string customerID, string lsStyle, DateTime fromDate, DateTime toDate);
    }
}
