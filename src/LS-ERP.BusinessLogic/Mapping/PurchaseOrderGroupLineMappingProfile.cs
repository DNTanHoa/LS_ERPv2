using AutoMapper;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Mapping
{
    public class PurchaseOrderGroupLineMappingProfile : Profile
    {
        public PurchaseOrderGroupLineMappingProfile()
        {
            CreateMap<PurchaseOrderGroupLine, PurchaseOrderGroupLineDtos>();
            CreateMap<PurchaseOrderLine, PurchaseOrderGroupLine>()
                .ForMember(x => x.ID, y => y.Ignore())
                .ForMember(x => x.UnitID, y => y.MapFrom(s => s.SecondUnitID));
        }
    }
}
