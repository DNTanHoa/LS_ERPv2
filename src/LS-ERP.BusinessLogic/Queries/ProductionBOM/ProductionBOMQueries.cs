
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
    public class ProductionBOMQueries : IProductionBOMQueries
    {
        
        private readonly SqlServerAppDbContext context;
        private readonly IConfiguration configuration;

        public ProductionBOMQueries(SqlServerAppDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public IEnumerable<FabricItemDtos> GetFabricItems(string mergeBlockLSStyle)
        {
            var connectionString = configuration.GetSection("ConnectionString")
               .GetSection("DbConnection").Value ?? string.Empty;
            SqlParameter[] parameters =
            {
                new SqlParameter("@MergeBlockLSStyle",mergeBlockLSStyle ?? string.Empty),               
            };
            DataTable table = SqlHelper.FillByReader(connectionString,
                "sp_SelectFabricItems", parameters);
            var result = table.AsListObject<FabricItemDtos>();
            foreach (var item in result)
            {
                if (item.CustomerID == "GA")
                {
                    if (item.PerUnitID == "LB")
                    {
                        if (item.FabricWidth != 0)
                        {
                            var a = item.QuantityPerUnit * 1000;
                            var b = item.FabricWeight;
                            var c = item.FabricWidth + 2;
                            //var c = 51 + 2;
                            decimal const1 = 21.53M;
                            decimal const2 = 0.45359237M;
                            //item.QuantityPerUnit = a / (b * c / 2 / const1) * const2 / 12;
                            decimal cons = a / (b * c / 2 / const1) * const2;
                            item.QuantityPerUnit = decimal.Round(cons, 4);

                        }
                    }
                }
            }
            return result;
        }
        
    }
}
