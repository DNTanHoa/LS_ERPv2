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
    public class PartMaterialMappingProfile : Profile
    {
        public PartMaterialMappingProfile()
        {
            CreateMap<PartMaterial, PartMaterialDtos>();
        }
    }
}
