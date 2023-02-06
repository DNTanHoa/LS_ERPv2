using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Mapping
{
    public class ShippingPlanMappingProfile : Profile
    {
        public ShippingPlanMappingProfile()
        {
            CreateMap<CreateShippingPlanCommand, ShippingPlan>();
            CreateMap<UpdateShippingPlanCommand, ShippingPlan>()
                .ForMember(x => x.ID, y => y.Ignore())
                .ForMember(x => x.Details, y => y.Ignore());
        }
    }
}
