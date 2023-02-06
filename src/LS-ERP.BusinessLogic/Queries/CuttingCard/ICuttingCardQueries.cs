using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface ICuttingCardQueries
    {
        public IEnumerable<CuttingCardDtos> GetByCuttingOutputID(int ID);
        public IEnumerable<CuttingCardDtos> GetByMasterID(string parrentID);
        public IEnumerable<CuttingCardDtos> GetByMultiMasterID(List<string> listParrentID);
        public IEnumerable<CuttingCardDtos> GetByID(string ID);
        public IEnumerable<CuttingCardDtos> GetLocationByID(string ID);
        public IEnumerable<CuttingCardDtos> GetByCompanyID(string companyId, string operation, string keyword, DateTime produceDate);
        public IEnumerable<ComplingReportStatusDtos> GetComplingReportStatus(string companyID, string customerName, string keyword, DateTime fromDate,DateTime toDate);
        public IEnumerable<CuttingCardDtos> GetMasterByCompanyID(string companyId, string operation, string keyword, DateTime produceDate);
        public IEnumerable<CuttingCardDtos> GetByCompanyIDForSupperMarket(string companyId, string operation, string keyword);
    }
}
