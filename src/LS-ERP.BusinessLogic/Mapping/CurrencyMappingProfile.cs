using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos.Currency;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Mapping
{
    public class CurrencyMappingProfile : Profile
    {
        public CurrencyMappingProfile()
        {
            CreateMap<Currency, CurrencyDtos>();
            CreateMap<CreateCurrencyCommand, Currency>();
            CreateMap<UpdateCurrencyCommand, Currency>();
        }
    }
}
