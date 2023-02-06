using AutoMapper;
using AutoMapper.Configuration;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class PartPriceQueries : IPartPriceQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public PartPriceQueries(SqlServerAppDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IEnumerable<PartPriceDtos> Get(string companyId, string styleNo)
        {
            var result = context.PartPrice.Where(x => x.CompanyID == companyId
                                             && (x.StyleNO == styleNo || string.IsNullOrEmpty(styleNo)))
                                           .OrderByDescending(x => x.CreatedAt).ToList();
            return result.Select(x => mapper.Map<PartPriceDtos>(x));
        }
        public IEnumerable<PartPriceDtos> GetById(int Id)
        {
            var result = context.PartPrice.Where(x => x.ID == Id);
            return result.Select(x => mapper.Map<PartPriceDtos>(x));
        }
        public IEnumerable<PartPriceDtos> GetWithDailyTarget(string companyId, string styleNo)
        {
            var result = context.PartPrice.Where(x => x.CompanyID == companyId
                                              && x.StyleNO == styleNo)
                                           .OrderByDescending(x => x.CreatedAt).ToList();
            return result.Select(x => mapper.Map<PartPriceDtos>(x));
        }
        public IEnumerable<PartPriceDtos> GetWithDailyTarget(string companyId, string styleNo, string item)
        {
            var result = context.PartPrice.Where(x => x.CompanyID == companyId
                                                 && x.StyleNO == styleNo
                                                 && x.Item == item);
            return result.Select(x => mapper.Map<PartPriceDtos>(x));
        }
    }
}
