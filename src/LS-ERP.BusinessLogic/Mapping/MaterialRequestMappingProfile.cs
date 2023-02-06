using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Mapping
{
    public class MaterialRequestMappingProfile : Profile
    {
        public MaterialRequestMappingProfile()
        {
            CreateMap<MaterialRequestDetail, IssuedLine>()
                .ForMember(x => x.ID, y => y.Ignore())
                .ForMember(x => x.ReceivedQuantity, y => y.MapFrom(s => s.Quantity))
                .ForMember(x => x.IssuedQuantity, y => y.MapFrom(s => s.Quantity));
        }
    }
}
