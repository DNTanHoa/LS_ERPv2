using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
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
    public class StorageDetailQueries : IStorageDetailQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IConfiguration configuration;

        public StorageDetailQueries(SqlServerAppDbContext context,
            IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }
        public List<StorageDetail> GetAllStorageDetails(string storageCode)
        {
            throw new NotImplementedException();
        }

        public List<StorageDetail> GetAllStorageDetails(string itemID, string customerID)
        {
            throw new NotImplementedException();
        }

        public List<StorageDetail> GetStorageDetails()
        {
            throw new NotImplementedException();
        }

        public List<StorageDetailDto> GetSummaryStorageDetailReports(string customerID, string storageCode, DateTime? fromDate, DateTime? toDate,
            string productionMethodCode, decimal onHandQuantity)
        {
            var connectionString = configuration.GetSection("ConnectionString")
                .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@CustomerID", customerID),
                new SqlParameter("@StorageCode",storageCode),
                new SqlParameter("@ProductionMethodCode",productionMethodCode),
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate",toDate),
                new SqlParameter("@OnHandQuantity",onHandQuantity)
            };

            if (customerID == null)
            {
                parameters[0].Value = DBNull.Value;
                parameters[0].Direction = ParameterDirection.Input;
            }

            if (storageCode == null)
            {
                parameters[1].Value = DBNull.Value;
                parameters[1].Direction = ParameterDirection.Input;
            }

            if (productionMethodCode == null)
            {
                parameters[2].Value = DBNull.Value;
                parameters[2].Direction = ParameterDirection.Input;
            }

            if (fromDate == null)
            {
                parameters[3].Value = DBNull.Value;
                parameters[3].Direction = ParameterDirection.Input;
            }

            if (toDate == null)
            {
                parameters[4].Value = DBNull.Value;
                parameters[4].Direction = ParameterDirection.Input;
            }

            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_SelectSummaryStorageDetailReport", parameters);
            return table.AsListObject<StorageDetailDto>();
        }
    }
}
