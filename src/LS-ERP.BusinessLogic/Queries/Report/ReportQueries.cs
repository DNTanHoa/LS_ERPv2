using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultils.Extensions;
using Ultils.Helpers;

namespace LS_ERP.BusinessLogic.Queries
{
    public class ReportQueries : IReportQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IConfiguration configuration;

        public ReportQueries(SqlServerAppDbContext context,
            IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public List<InventoryReportDto> GetInventoryReport(string purchaseOrder = "", string search = "", 
            string storageCode = "", string customerID = "")
        {
            var connectionString = configuration.GetSection("ConnectionString")
                .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@PurchaseOrder", purchaseOrder),
                new SqlParameter("@StorageCode",storageCode),
                new SqlParameter("@Search",search),
                new SqlParameter("@customerID",customerID)
            };
            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_SelectInventoryReport", parameters);
            return table.AsListObject<InventoryReportDto>();
        }

        public List<IssuedReportDto> GetIssuedReports(string CustomerID, string StorageCode, 
            DateTime fromDate, DateTime toDate)
        {
            var connectionString = configuration.GetSection("ConnectionString")
                .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@CustomerID",CustomerID),
                new SqlParameter("@StorageCode",StorageCode),
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };
            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_SelectIssuedReport", parameters);
            return table.AsListObject<IssuedReportDto>();
        }

        public List<ItemReportDto> GetItemReportDtos(string style, string customerID, 
            DateTime fromDate, DateTime toDate)
        {
            var connectionString = configuration.GetSection("ConnectionString")
                .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@CustomerID",customerID),
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", toDate)
            };
            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_SelectSummaryItemReport", parameters);
            return table.AsListObject<ItemReportDto>();
        }

        public List<SeasonReportDto> GetSeasonReport(string customerID, string style, string season, string keyword)
        {
            var connectionString = configuration.GetSection("ConnectionString")
                .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@CustomerID",customerID ?? string.Empty),
                new SqlParameter("@Style",style ?? string.Empty),
                new SqlParameter("@Season", season ?? string.Empty),
                new SqlParameter("@Keyword", keyword ?? string.Empty)
            };
            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_SelectSummary", parameters);
            return table.AsListObject<SeasonReportDto>();
        }
    }
}
