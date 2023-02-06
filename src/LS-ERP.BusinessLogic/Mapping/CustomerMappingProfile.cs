using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos.Customer;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Mapping
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<Customer, CustomerSummaryDtos>()
                .ForMember(x => x.Division, y => y.MapFrom(s => s.Division.Name))
                .ForMember(x => x.Currency, y => y.MapFrom(s => s.Currency.ID))
                .ForMember(x => x.PaymentTerm, y => y.MapFrom(s => s.PaymentTerm.Description))
                .ForMember(x => x.PriceTerm, y => y.MapFrom(s => s.PriceTerm.Description));
            CreateMap<Customer, CustomerDetailDtos>()
                .ForMember(x => x.Division, y => y.MapFrom(s => s.Division.Name))
                .ForMember(x => x.Currency, y => y.MapFrom(s => s.Currency.ID))
                .ForMember(x => x.PaymentTerm, y => y.MapFrom(s => s.PaymentTerm.Description))
                .ForMember(x => x.PriceTerm, y => y.MapFrom(s => s.PriceTerm.Description));
            CreateMap<CreateCustomerCommand, Customer>();
            CreateMap<UpdateCustomerCommand, Customer>();
        }
    }
}
