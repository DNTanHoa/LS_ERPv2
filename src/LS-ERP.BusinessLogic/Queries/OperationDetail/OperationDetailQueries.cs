using AutoMapper;
using Microsoft.Extensions.Configuration;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using Microsoft.EntityFrameworkCore;
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
    public class OperationDetailQueries : IOperationDetailQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        public OperationDetailQueries(SqlServerAppDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public IEnumerable<OperationDetailDtos> GetByID(string ID)
        {
            var result = context.OperationDetail.Where(x => x.ID == ID).ToList();

            return result.Select(x => mapper.Map<OperationDetailDtos>(x)).OrderBy(x => x.MergeBlockLSStyle);
        }
        public IEnumerable<OperationDetailDtos> GetAll()
        {
            var result = context.OperationDetail.ToList();

            return result.Select(x => mapper.Map<OperationDetailDtos>(x)).OrderBy(x => x.MergeBlockLSStyle);
        }
      
        

    }   
}
