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
    public class WorkCenterQueries : IWorkCenterQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        public WorkCenterQueries(SqlServerAppDbContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IEnumerable<WorkCenterDtos> Get(string departmentId)
        {
            var result = context.WorkCenter.Where(x => x.DepartmentID == departmentId);
            return result.Select(x=>mapper.Map<WorkCenterDtos>(x));
        }
        public IEnumerable<WorkCenterDtos> Get()
        {
            var result = context.WorkCenter.OrderBy(x=>x.PlantCode).ThenBy(x=>x.SortIndex);
            return result.Select(x => mapper.Map<WorkCenterDtos>(x));
        }
        public IEnumerable<WorkCenterDtos> Get(List<string> listWorkCenterID)
        {
            var result = context.WorkCenter.Where(x=>listWorkCenterID.Contains(x.ID)).OrderBy(o => o.ID);
            return result.Select(x => mapper.Map<WorkCenterDtos>(x));
        }
        public IEnumerable<WorkCenterDtos> GetCuttingCenterByCompany(string companyID)
        {
            var result = context.WorkCenter.Where(x => x.ID.Contains(companyID)
                                                    && x.WorkCenterTypeID== "CUTTING"
                                                    ).ToList();
            return result.Select(x => mapper.Map<WorkCenterDtos>(x));
        }
        public IEnumerable<WorkCenterDtos> GetSewingCenterByCompany(string companyID)
        {
            var result = context.WorkCenter.Where(x => x.ID.Contains(companyID)
                                                    && x.WorkCenterTypeID == "SEWING"
                                                    ).OrderBy(x=>x.PlantCode).ThenBy(x=>x.SortIndex).ToList();
            return result.Select(x => mapper.Map<WorkCenterDtos>(x));
        }
    }
}
