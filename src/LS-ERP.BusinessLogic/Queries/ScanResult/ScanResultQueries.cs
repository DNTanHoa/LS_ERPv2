using Castle.Core.Resource;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
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
    public class ScanResultQueries : IScanResultQueries
    {
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;

        public ScanResultQueries(AppDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public IEnumerable<ScanResultDto> GetSummaryScanResult(string company, DateTime summaryDate)
        {
            var connectionString = configuration.GetSection("ConnectionString")
               .GetSection("DbConnection").Value ?? string.Empty;

            SqlParameter[] parameters =
            {
                new SqlParameter("@Company", company ?? string.Empty),
                new SqlParameter("@SummaryDate",summaryDate),
            };

            DataTable table = SqlHelper.FillByReader(connectionString, "sp_SelectSummaryScanResult", parameters);
            var result = table.AsListObject<ScanResultDto>();

            return result;
        }
    }
}

