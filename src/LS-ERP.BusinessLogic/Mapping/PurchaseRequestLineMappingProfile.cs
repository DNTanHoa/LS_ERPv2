using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Mapping
{
    public class PurchaseRequestLineMappingProfile : Profile
    {
        public PurchaseRequestLineMappingProfile()
        {
            CreateMap<ProductionBOM, PurchaseRequestLine>()
                .ForMember(x => x.ID, y => y.Ignore())
                .ForMember(x => x.Quantity, y => y.MapFrom(s => s.RequiredQuantity))
                .ForMember(x => x.CustomerStyle, y => y.MapFrom(s => s.ItemStyle.CustomerStyle))
                .ForMember(x => x.GarmentColorCode, y => y.MapFrom(s => s.ItemStyle.ColorCode))
                .ForMember(x => x.Season, y => y.MapFrom(s => s.ItemStyle.Season))
                .ForMember(x => x.UnitID, y => y.MapFrom(s => s.PerUnitID))
                .ForMember(x => x.GarmentColorName, y => y.MapFrom(s => s.ItemStyle.ColorName))
                .ForMember(x => x.ContractNo, y => y.MapFrom(s => s.ItemStyle.ContractNo))
                .ForMember(x => x.LSStyle, y => y.MapFrom(s => s.ItemStyle.LSStyle));
        }
    }
}
