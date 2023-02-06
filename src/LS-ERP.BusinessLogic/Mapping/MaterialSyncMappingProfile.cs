using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Mapping
{
    public class MaterialSyncMappingProfile : Profile
    {
        public MaterialSyncMappingProfile()
        {
            CreateMap<ProductionBOM, MaterialSync>()
                .ForMember(x => x.ID, y => y.Ignore())
                .ForMember(x => x.ProductionBOMID, y => y.MapFrom(s => s.ID));
        }
    }
}
