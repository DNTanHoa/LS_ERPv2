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
    public class IssuedMappingProfile : Profile
    {
        public IssuedMappingProfile()
        {
            CreateMap<CreateIssuedCommand, Issued>();
            CreateMap<CreateIssuedFabricCommand, Issued>();
            CreateMap<UpdateIssuedCommand, Issued>()
                .ForMember(x => x.IssuedLines, y => y.Ignore())
                .ForMember(x => x.IssuedGroupLines, y => y.Ignore());
        }
    }
}
