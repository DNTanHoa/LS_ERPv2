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
    public class OperationQueries : IOperationQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        public OperationQueries(SqlServerAppDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public IEnumerable<OperationDtos> GetByID(string ID)
        {
            var result = context.Operation.Where(x => x.ID == ID).ToList();

            return result.Select(x => mapper.Map<OperationDtos>(x)).OrderBy(x => x.Name);
        }
        public IEnumerable<OperationDtos> GetAll()
        {
            var result = context.Operation.ToList();

            return result.Select(x => mapper.Map<OperationDtos>(x)).OrderBy(x => x.Name);
        }
        public IEnumerable<OperationDtos> GetByGroup(string group)
        {
            var result = context.Operation.Where(x => x.Group == group && !x.Name.Equals("MASTER")).OrderBy(x=>x.Name).ToList();
          
            return result.Select(x => mapper.Map<OperationDtos>(x)).OrderBy(x=>x.Name);
        }
        

    }   
}
