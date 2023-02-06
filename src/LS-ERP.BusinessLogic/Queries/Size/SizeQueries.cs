using AutoMapper;
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
    public class SizeQueries : ISizeQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        public SizeQueries(SqlServerAppDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
      
        public IEnumerable<SizeDtos> GetByCustomerId(string customerID)
        {
            var result = context.Size.Where(x => x.CustomerID == customerID).OrderBy(x=>x.Code).ToList();
            return result.Select(x => mapper.Map<SizeDtos>(x));
        }

        public IEnumerable<SizeLSStyleDtos> GetByLSStyle(string lsStyle)
        {
            var result = context.OrderDetail.Where(o => o.ItemStyle.LSStyle == lsStyle).ToList().OrderBy(o=>o.Size);
           
            return result.Select(x => mapper.Map<SizeLSStyleDtos>(x));
        }

    }
}
