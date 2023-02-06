using AutoMapper;
using LS_ERP.BusinessLogic.Dtos.PriceTerm;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class PriceTermQueries : IPriceTermQueries
    {
        private readonly IPriceTermRepository priceTermRepository;
        private readonly IMapper mapper;

        public PriceTermQueries(IPriceTermRepository priceTermRepository,
            IMapper mapper)
        {
            this.priceTermRepository = priceTermRepository;
            this.mapper = mapper;
        }

        public PriceTermDtos GetPriceTerm(string Code)
        {
            var priceTerm = priceTermRepository.GetPriceTerm(Code);

            if(priceTerm != null)
            {
                return mapper.Map<PriceTermDtos>(priceTerm);
            }

            return null;
        }

        public IEnumerable<PriceTermDtos> GetPriceTerms()
        {
            var priceTerms = priceTermRepository.GetPriceTerms();
            return priceTerms.Select(x => mapper.Map<PriceTermDtos>(x));
        }
    }
}
