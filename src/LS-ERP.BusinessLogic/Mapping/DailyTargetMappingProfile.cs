using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Mapping
{
    public class DailyTargetMappingProfile : Profile
    {
        public DailyTargetMappingProfile()
        {
            CreateMap<CreateDailyTargetCommand, DailyTarget>();
            CreateMap<UpdateDailyTargetCommand, DailyTarget>();
            CreateMap<DailyTargetModel, DailyTarget>();
            CreateMap<DailyTarget, JobOutput>().ForMember(x=>x.ID,y=>y.Ignore());
            CreateMap<DailyTarget, AllocDailyOutput>().ForMember(x => x.ID, y => y.Ignore());
            CreateMap<DailyTargetModel, AllocDailyOutput>().ForMember(x => x.ID, y => y.Ignore()).ForMember(x=>x.OrderQuantity, y=>y.MapFrom(x=>x.TargetQuantity));
            CreateMap<DailyTarget, DailyTargetDtos>();
            CreateMap<DailyTarget,DailyTagetOverviewDtos>();
        }
    }
}
