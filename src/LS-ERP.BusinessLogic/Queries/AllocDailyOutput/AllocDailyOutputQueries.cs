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
    public class AllocDailyOutputQueries : IAllocDailyOutputQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        public AllocDailyOutputQueries(SqlServerAppDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public IEnumerable<AllocDailyOutputDtos> GetByTargetID(int ID)
        {
            var result = context.AllocDailyOutput.Where(x=>x.TargetID == ID).ToList();
            return result.Select(x => mapper.Map<AllocDailyOutputDtos>(x)).ToList().OrderBy(o => o.FabricContrastName).ThenBy(o => o.Set).ThenBy(o => o.Size).ToList(); 
        }
        public IEnumerable<AllocDailyOutputDtos> GetByID(int ID)
        {
            var result = context.AllocDailyOutput.Where(x => x.ID == ID).ToList();
            return result.Select(x => mapper.Map<AllocDailyOutputDtos>(x)).ToList();
        }
    }   
}
