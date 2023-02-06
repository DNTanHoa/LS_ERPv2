using LS_ERP.EntityFrameworkCore.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class LoadingPlanQueries : ILoadingPlanQueries
    {
        private readonly IConfiguration configuration;

        public LoadingPlanQueries(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public List<LoadingPlan> GetAll()
        {
            string tableName = typeof(LoadingPlan).Name;
            var connectionString = configuration
                .GetSection("ConnectionString").GetSection("DbConnection").Value;

            using (var db = new QueryFactory(
                   new SqlConnection(connectionString), new SqlServerCompiler()))
            {
                IEnumerable<LoadingPlan> entities = db.Query(tableName)
                    .Get<LoadingPlan>();

                return entities.ToList();
            }
        }
    }
}
