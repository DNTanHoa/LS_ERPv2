using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IReportQueries
    {
        public List<ItemReportDto> GetItemReportDtos(string style, string customerID,
            DateTime fromDate, DateTime toDate);

        public List<SeasonReportDto> GetSeasonReport(string customerID, string style, 
            string season, string keyword);

        public List<IssuedReportDto> GetIssuedReports(string CustomerID, string StorageCode,
            DateTime fromDate, DateTime toDate);

        public List<InventoryReportDto> GetInventoryReport(string purchaseOrder = "", string search = "",
            string storageCode = "", string customerID = "");
    }
}
