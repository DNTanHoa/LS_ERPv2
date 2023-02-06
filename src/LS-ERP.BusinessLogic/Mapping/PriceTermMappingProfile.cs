using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos.PriceTerm;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Mapping
{
    public class PriceTermMappingProfile : Profile
    {
        public PriceTermMappingProfile()
        {
            CreateMap<PriceTerm, PriceTermDtos>();
            CreateMap<CreatePriceTermCommand, PriceTerm>();
            CreateMap<UpdatePriceTermCommand, PriceTerm>();
        }
    }
}
