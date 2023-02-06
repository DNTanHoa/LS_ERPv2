using AutoMapper;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class ProblemQueries : IProblemQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        public ProblemQueries(SqlServerAppDbContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IEnumerable<ProblemDtos> GetAll()
        {
            var result =  context.Problem;
            return result.Select(x=>mapper.Map<ProblemDtos>(x));
        }
        
    }
}
