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
    public class ReceiptQueries : IReceiptQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public ReceiptQueries(SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public List<ReceiptDto> GetReceiptSummary(string numbers, string storageCode, DateTime fromDate, DateTime toDate)
        {
            var result = new List<ReceiptDto>();
            var data = context.ReceiptGroupLine.Include(x => x.Receipt)
                .Where(x => (x.Receipt.Number.Contains(numbers) || string.IsNullOrEmpty(numbers)) &&
                            x.Receipt.IsDeleted != true &&
                            x.Receipt.StorageCode == storageCode &&
                            x.Receipt.ReceiptDate >= fromDate &&
                            x.Receipt.ReceiptDate <= toDate);
            result = data.Select(x => mapper.Map<ReceiptDto>(x)).ToList();
            return result;
        }
    }
}
