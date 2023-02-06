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
    public class PurchaseRequestMappingProfile : Profile
    {
        public PurchaseRequestMappingProfile()
        {
            CreateMap<SavePurchaseRequestCommand, PurchaseRequest>();
        }
    }
}
