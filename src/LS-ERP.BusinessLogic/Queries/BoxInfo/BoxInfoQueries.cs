using Castle.Core.Resource;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Entities;
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
    public class BoxInfoQueries : IBoxInfoQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IConfiguration configuration;

        public BoxInfoQueries(SqlServerAppDbContext context,
            IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public Task<List<BoxInfo>> GetAll()
        {
            var connectionString = configuration.GetSection("ConnectionString")
                .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@Search",string.Empty),
                new SqlParameter("@customerID",string.Empty)
            };
            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_LoadBoxInfo", parameters);
            return Task.FromResult(table.AsListObject<BoxInfo>());
        }
    }
}
