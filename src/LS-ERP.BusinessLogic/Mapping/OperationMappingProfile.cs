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
    public class OperationMappingProfile : Profile
    {
        public OperationMappingProfile()
        {
            CreateMap<Operation, OperationDtos>();
            CreateMap<CreateOperationCommand, Operation>().ForMember(x => x.ID, y => y.Ignore());
            CreateMap<UpdateOperationCommand, Operation>();
        }
    }
}
