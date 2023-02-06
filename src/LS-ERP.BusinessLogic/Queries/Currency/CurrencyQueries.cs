using AutoMapper;
using LS_ERP.BusinessLogic.Dtos.Currency;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class CurrencyQueries : ICurrencyQueries
    {
        private readonly ICurrencyRepository currencyRepository;
        private readonly IMapper mapper;

        public CurrencyQueries(ICurrencyRepository currencyRepository,
            IMapper mapper)
        {
            this.currencyRepository = currencyRepository;
            this.mapper = mapper;
        }

        public IEnumerable<CurrencyDtos> GetCurrencies()
        {
            var currencies = currencyRepository.GetCurrencys();
            return currencies.Select(x => mapper.Map<CurrencyDtos>(x));
        }

        public CurrencyDtos GetCurrency(string ID)
        {
            var currency = currencyRepository.GetCurrency(ID);
            if(currency != null)
            {
                return mapper.Map<CurrencyDtos>(currency);
            }
            return null;
        }
    }
}
