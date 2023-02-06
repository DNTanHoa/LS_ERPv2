using AutoMapper;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class JobPriceQueries : IJobPriceQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public JobPriceQueries(SqlServerAppDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IEnumerable<JobPriceDtos> Get(string companyId)
        {
            var result = context.JobPrice.Where(x => x.CompanyID == companyId);
            return result.Select(x => mapper.Map<JobPriceDtos>(x));
        }

        public IEnumerable<JobPriceDtos> GetById(int Id)
        {
            var result = context.JobPrice.Where(x => x.ID == Id);
            return result.Select(x => mapper.Map<JobPriceDtos>(x));
        }
    }
}
