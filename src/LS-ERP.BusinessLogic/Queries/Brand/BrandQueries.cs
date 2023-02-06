using AutoMapper;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class BrandQueries : IBrandQueries
    {
        private readonly IBrandRepository brandRepository;
        private readonly IMapper mapper;

        public BrandQueries(IBrandRepository brandRepository,
            IMapper mapper)
        {
            this.brandRepository = brandRepository;
            this.mapper = mapper;
        }

        public BrandDtos GetBrand(string Code)
        {
            var brand = brandRepository.GetBrand(Code);

            if (brand != null)
            {
                return mapper.Map<BrandDtos>(brand);
            }

            return null;
        }

        public IEnumerable<BrandDtos> GetBrands()
        {
            return brandRepository.GetBrands()
                .Select(x => mapper.Map<BrandDtos>(x));
        }

        public IEnumerable<BrandDtos> GetBrandsByCustomer(string CustomerID)
        {
            return brandRepository.GetBrands()
                .Where(x => x.CustomerID == CustomerID)
                .Select(x => mapper.Map<BrandDtos>(x));
        }
    }
}
