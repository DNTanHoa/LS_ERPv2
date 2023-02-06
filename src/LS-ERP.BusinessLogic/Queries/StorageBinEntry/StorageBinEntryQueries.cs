using AutoMapper;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class StorageBinEntryQueries : IStorageBinEntryQueries
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;

        public StorageBinEntryQueries(AppDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
  

        public IEnumerable<StorageBinEntryDtos> GetStorageBinEntry(string storageCode)
        {
            var result = context.StorageBinEntry.Where(s => s.StorageCode == storageCode).OrderBy(s => s.BinCode).ToList();
            return result.Select(x => mapper.Map<StorageBinEntryDtos>(x)).ToList();
        }
    }
}
