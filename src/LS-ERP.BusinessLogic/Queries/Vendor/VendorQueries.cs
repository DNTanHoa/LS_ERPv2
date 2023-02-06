using AutoMapper;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class VendorQueries : IVendorQueries
    {
        private readonly SqlServerAppDbContext context;
        private readonly IVendorRepository vendorRepository;
        private readonly IMapper mapper;

        public VendorQueries(SqlServerAppDbContext context,IVendorRepository vendorRepository,
            IMapper mapper)
        {
            this.context = context;
            this.vendorRepository = vendorRepository;
            this.mapper = mapper;
        }

        public IEnumerable<VendorSelectDtos> GetSelectVendor()
        {
            return vendorRepository.GetVendors()
                .Select(x => mapper.Map<VendorSelectDtos>(x));
        }

        public VendorDtos GetVendor(string ID)
        {
            var vendor = vendorRepository.GetVendor(ID);

            if(vendor != null)
            {
                return mapper.Map<VendorDtos>(vendor);
            }

            return null;
        }

        public IEnumerable<VendorDtos> GetVendors()
        {
            return vendorRepository.GetVendors()
                .Select(x => mapper.Map<VendorDtos>(x));
        }
        public IEnumerable<VendorDtos> GetByDescription(string description)
        {
            return context.Vendor.Where(v=>v.Description== "PRINT")
                .Select(x => mapper.Map<VendorDtos>(x));
        }
    }
}
