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
    public class FabricRequestMappingProfile : Profile
    {
        public FabricRequestMappingProfile()
        {
            CreateMap<SaveFabricRequestCommand, FabricRequest>().ForMember(x => x.ID, y => y.Ignore());
            CreateMap<UpdateFabricRequestCommand, FabricRequest>();
            CreateMap<FabricRequest, FabricRequest>().ForMember(x => x.Details, y => y.Ignore())
                                                     .ForMember(x => x.CreatedAt, y => y.Ignore())
                                                     .ForMember(x => x.CreatedBy, y => y.Ignore())
                                                     .ForMember(x => x.LastUpdatedAt, y => y.Ignore())
                                                     .ForMember(x => x.LastUpdatedBy, y => y.Ignore());
        }
    }
}
