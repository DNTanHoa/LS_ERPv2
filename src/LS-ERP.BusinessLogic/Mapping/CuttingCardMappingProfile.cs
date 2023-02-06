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
    public class CuttingCardMappingProfile : Profile
    {
        public CuttingCardMappingProfile()
        {
            CreateMap<CuttingCard, CuttingCardDtos>();
            CreateMap<CuttingCard, CuttingCard>().ForMember(x => x.ID, y => y.Ignore());
            CreateMap<CreateCuttingCardCommand, CuttingCard>().ForMember(x=>x.ID,y=>y.Ignore());
            CreateMap<UpdateCuttingCardCommand, CuttingCard>();
            CreateMap<CuttingCard, DeliveryNoteDetail>().ForMember(x => x.ID,y => y.Ignore());
           
        }
    }
}
