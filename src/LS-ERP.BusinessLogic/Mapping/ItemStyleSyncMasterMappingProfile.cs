using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Mapping
{
    public class ItemStyleSyncMasterMappingProfile : Profile
    {
        public ItemStyleSyncMasterMappingProfile()
        {
            CreateMap<ItemStyle, ItemStyleSyncMaster>()
                .ForMember(x => x.Brand, y => y.MapFrom(s => s.SalesOrder.BrandCode))
                .ForMember(x => x.CustomerID, y => y.MapFrom(s => s.SalesOrder.CustomerID))
                .ForMember(x => x.Merchandiser, y => y.MapFrom(s => s.SalesOrder.CreatedBy))
                .ForMember(x => x.ItemStyleNumber, y => y.MapFrom(s => s.Number))
                .ForMember(x => x.ProductType, y => y.MapFrom(s => s.GetProductType()))
                .ForMember(x => x.CreatedBy, y => y.MapFrom(s => s.SalesOrder.CreatedBy))
                .ForMember(x => x.CreatedAt, y => y.MapFrom(s => s.SalesOrder.CreatedAt))
                .ForMember(x => x.ContractualSupplierHandover, y => y.MapFrom(s => s.ContractDate))
                .ForMember(x => x.LastUpdatedBy, y => y.MapFrom(s => s.SalesOrder.LastUpdatedBy))
                .ForMember(x => x.LastUpdatedAt, y => y.MapFrom(s => s.SalesOrder.LastUpdatedAt));
        }
    }
}
