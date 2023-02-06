using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos.SalesOrder;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Mapping
{
    public class SalesOrderMappingProfile : Profile
    {
        public SalesOrderMappingProfile()
        {
            CreateMap<SalesOrder, SalesOrderSummaryDtos>()
                .ForMember(x => x.Customer, y => y.MapFrom(s => s.Customer.Name))
                .ForMember(x => x.PaymentTerm, y => y.MapFrom(s => s.PaymentTerm.Description))
                .ForMember(x => x.PriceTerm, y => y.MapFrom(s => s.PriceTerm.Description))
                .ForMember(x => x.SalesOrderType, y => y.MapFrom(s => s.SalesOrderType.Name))
                .ForMember(x => x.SalesOrderStatus, y => y.MapFrom(s => s.SalesOrderStatus.Name))
                .ForMember(x => x.Brand, y => y.MapFrom(s => s.Brand.Name))
                .ForMember(x => x.Currency, y => y.MapFrom(s => s.Currency.Description))
                .ForMember(x => x.Division, y => y.MapFrom(s => s.Division.Name));

            CreateMap<SalesOrder, SalesOrderDetailDtos>()
                .ForMember(x => x.Customer, y => y.MapFrom(s => s.Customer.Name))
                .ForMember(x => x.PaymentTerm, y => y.MapFrom(s => s.PaymentTerm.Description))
                .ForMember(x => x.PriceTerm, y => y.MapFrom(s => s.PriceTerm.Description))
                .ForMember(x => x.Brand, y => y.MapFrom(s => s.Brand.Name))
                .ForMember(x => x.Currency, y => y.MapFrom(s => s.Currency.Description))
                .ForMember(x => x.SalesOrderType, y => y.MapFrom(s => s.SalesOrderType.Name))
                .ForMember(x => x.SalesOrderStatus, y => y.MapFrom(s => s.SalesOrderStatus.Name))
                .ForMember(x => x.Division, y => y.MapFrom(s => s.Division.Name));

            CreateMap<CreateSalesOrderCommand, SalesOrder>();
            CreateMap<UpdateSalesOrderCommand, SalesOrder>();
        }
    }
}
