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
    public class ReceiptMappingProfile : Profile
    {
        public ReceiptMappingProfile()
        {
            CreateMap<CreateReceiptCommand, Receipt>();
            CreateMap<CreateReceiptFabricCommand, Receipt>();
            CreateMap<UpdateReceiptCommand, Receipt>();
            CreateMap<ReceiptGroupLine, ReceiptDto>()
                .ForMember(x => x.PurchaseOrderNumber, y => y.MapFrom(s => s.Receipt.PurchaseOrderNumber))
                .ForMember(x => x.FabricPurchaseOrderNumber, y => y.MapFrom(s => s.Receipt.FabricPurchaseOrderNumber))
                .ForMember(x => x.InvoiceNumber, y => y.MapFrom(s => s.Receipt.InvoiceNumber))
                .ForMember(x => x.InvoiceNumberNoTotal, y => y.MapFrom(s => s.Receipt.InvoiceNumberNoTotal))
                .ForMember(x => x.CustomerID, y => y.MapFrom(s => s.Receipt.CustomerID))
                .ForMember(x => x.VendorID, y => y.MapFrom(s => s.Receipt.VendorID))
                .ForMember(x => x.StorageCode, y => y.MapFrom(s => s.Receipt.StorageCode))
                .ForMember(x => x.ReceiptDate, y => y.MapFrom(s => s.Receipt.ReceiptDate))
                .ForMember(x => x.ReceiptBy, y => y.MapFrom(s => s.Receipt.ReceiptBy))
                .ForMember(x => x.ReceiptNumber, y => y.MapFrom(s => s.Receipt.Number));
        }
    }
}
