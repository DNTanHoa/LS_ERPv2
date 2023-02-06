using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Mapping
{
    public class FabricRequestLogMappingProfile : Profile
    {
        public FabricRequestLogMappingProfile()
        {
            CreateMap<FabricRequest, FabricRequestLog>().ForMember(x => x.ID, y => y.Ignore())
                                                        .ForMember(x => x.FabricRequestID, y => y.MapFrom(s => s.ID))
                                                        .ForMember(x => x.Status, y => y.Ignore())
                                                        .ForMember(x => x.Customer, y => y.Ignore())
                                                        .ForMember(x => x.Company, y => y.Ignore())
                                                        .ForMember(x => x.Details, y => y.MapFrom(s => s.Details));
            CreateMap<FabricRequestDetail, FabricRequestDetailLog>()
                                                        .ForMember(x => x.ID, y => y.Ignore());
        }
    }
}
