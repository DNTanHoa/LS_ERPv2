using AutoMapper;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Mapping
{
    public class PurchaseOrderLineMappingProfile : Profile
    {
        public PurchaseOrderLineMappingProfile()
        {
            CreateMap<ProductionBOM, PurchaseOrderLine>()
                .ForMember(x => x.ID, y => y.Ignore())
                .ForMember(x => x.ReservedQuantity, y => y.Ignore())
                .ForMember(x => x.Quantity, y => y.Ignore())
                .ForMember(x => x.ReservedForecastQuantity, y => y.Ignore())
                .ForMember(x => x.CustomerStyle, y => y.MapFrom(s => s.ItemStyle.CustomerStyle))
                .ForMember(x => x.SalesOrderID, y => y.MapFrom(s => s.ItemStyle.SalesOrderID))
                .ForMember(x => x.Division, y => y.MapFrom(s => s.ItemStyle.Division))
                .ForMember(x => x.Label, y => y.MapFrom(s => s.ItemStyle.LabelCode)) ///TODO: Sai hậu sửa
                .ForMember(x => x.CustomerPurchaseOrderNumber, y => y.MapFrom(s => s.ItemStyle.PurchaseOrderNumber))
                .ForMember(x => x.GarmentColorCode, y => y.MapFrom(s => s.ItemStyle.ColorCode))
                .ForMember(x => x.Season, y => y.MapFrom(s => s.ItemStyle.Season))
                .ForMember(x => x.ShipDate, y => y.MapFrom(s => s.ItemStyle.ShipDate))
                .ForMember(x => x.UnitID, y => y.MapFrom(s => s.PerUnitID))
                .ForMember(x => x.SecondUnitID, y => y.MapFrom(s => s.PriceUnitID))
                .ForMember(x => x.GarmentColorName, y => y.MapFrom(s => s.ItemStyle.ColorName))
                .ForMember(x => x.ContractNo, y => y.MapFrom(s => s.ItemStyle.ContractNo))
                .ForMember(x => x.LSStyle, y => y.MapFrom(s => s.ItemStyle.LSStyle));

            CreateMap<PurchaseRequestLine, PurchaseOrderLine>()
               .ForMember(x => x.ID, y => y.Ignore())
               .ForMember(x => x.ReservedQuantity, y => y.Ignore())
               .ForMember(x => x.Quantity, y => y.Ignore())
               .ForMember(x => x.ReservedForecastQuantity, y => y.Ignore())
               .ForMember(x => x.CustomerStyle, y => y.MapFrom(s => s.CustomerStyle))
               .ForMember(x => x.PurchaseRequestID, y => y.MapFrom(s => s.PurchaseRequestID))
               .ForMember(x => x.GarmentColorCode, y => y.MapFrom(s => s.GarmentColorCode))
               .ForMember(x => x.Season, y => y.MapFrom(s => s.Season))
               .ForMember(x => x.UnitID, y => y.MapFrom(s => s.UnitID))
               .ForMember(x => x.SecondUnitID, y => y.MapFrom(s => s.PriceUnitID))
               .ForMember(x => x.GarmentColorName, y => y.MapFrom(s => s.GarmentColorName))
               .ForMember(x => x.LSStyle, y => y.MapFrom(s => s.LSStyle));

            CreateMap<ForecastMaterial, PurchaseOrderLine>()
                .ForMember(x => x.ID, y => y.Ignore())
                .ForMember(x => x.ReservedQuantity, y => y.Ignore())
                .ForMember(x => x.Quantity, y => y.Ignore())
                .ForMember(x => x.ReservedForecastQuantity, y => y.Ignore())
                .ForMember(x => x.CustomerStyle, y => y.MapFrom(s => s.ForecastOverall.CustomerStyle))
                .ForMember(x => x.ForecastWeekID, y => y.MapFrom(s => s.ForecastOverall.ForecastEntry.Title))
                .ForMember(x => x.ContractualHandoverDateWeekID, y => y.MapFrom(s => s.ForecastOverall.CreateWeekTitle))
                .ForMember(x => x.Division, y => y.MapFrom(s => s.ForecastOverall.Division))
                .ForMember(x => x.Label, y => y.MapFrom(s => s.ForecastOverall.LabelCode)) ///TODO: Sai hậu sửa
                .ForMember(x => x.CustomerPurchaseOrderNumber, y => y.MapFrom(s => s.ForecastOverall.PurchaseOrderNumber))
                .ForMember(x => x.GarmentColorCode, y => y.MapFrom(s => s.ForecastOverall.GarmentColorCode))
                .ForMember(x => x.Season, y => y.MapFrom(s => s.ForecastOverall.Season))
                .ForMember(x => x.ShipDate, y => y.MapFrom(s => s.ForecastOverall.ShipDate))
                .ForMember(x => x.UnitID, y => y.MapFrom(s => s.PerUnitID))
                .ForMember(x => x.SecondUnitID, y => y.MapFrom(s => s.PriceUnitID))
                .ForMember(x => x.GarmentColorName, y => y.MapFrom(s => s.ForecastOverall.GarmentColorName))
                .ForMember(x => x.ContractNo, y => y.MapFrom(s => s.ForecastOverall.ContractNo))
                .ForMember(x => x.LSStyle, y => y.MapFrom(s => s.ForecastOverall.LSCode));
        }
    }
}
