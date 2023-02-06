using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos.PaymentTerm;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Mapping
{
    public class PaymentTermMappingProfile : Profile
    {
        public PaymentTermMappingProfile()
        {
            CreateMap<PaymentTerm, PaymentTermDtos>();
            CreateMap<CreatePaymentTermCommand, PaymentTerm>();
            CreateMap<UpdatePaymentTermCommand, PaymentTerm>();
        }
    }
}
