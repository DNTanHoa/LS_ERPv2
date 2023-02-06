using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Dtos;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;
using System.Linq;
namespace LS_ERP.XAF.Module.Controllers
{
    public partial class UpPrice_ViewController : ViewController
    {
        public UpPrice_ViewController()
        {
            InitializeComponent();

            SimpleAction upPricePurchaseOrderLine = new SimpleAction(this, "UpPricePurchaseOrderLine", PredefinedCategory.Unspecified);
            upPricePurchaseOrderLine.ImageName = "MergeAcross";
            upPricePurchaseOrderLine.Caption = "Up Price";
            upPricePurchaseOrderLine.TargetObjectType = typeof(PurchaseOrderLine);
            upPricePurchaseOrderLine.TargetViewType = ViewType.ListView;
            upPricePurchaseOrderLine.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            upPricePurchaseOrderLine.Shortcut = "CtrlL";

            upPricePurchaseOrderLine.Execute += UpPricePurchaseOrderLine_Execute;
        }

        private void UpPricePurchaseOrderLine_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var purchaseOrder = (((ListView)View).CollectionSource as PropertyCollectionSource).MasterObject
               as PurchaseOrder;
            var service = new PurchaseOrderLineService();

            var config = new MapperConfiguration(c =>
            {
                c.CreateMap<ProductionBOM, ProductionBOMDto>()
                    .ForMember(x => x.GarmentColorName, y => y.MapFrom(s => s.ItemStyle.ColorName))
                    .ForMember(x => x.GarmentColorCode, y => y.MapFrom(s => s.ItemStyle.ColorCode));
                c.CreateMap<ItemStyle, ItemStyleDto>();
                c.CreateMap<PurchaseOrderLine, PurchaseOrderLineDto>();
                c.CreateMap<PurchaseOrderGroupLine, PurchaseOrderGroupLineDto>();
                c.CreateMap<ReservationEntry, ReservationEntryDto>();
                c.CreateMap<ReservationForecastEntry, ReservationForecastEntryDto>();
            });

            var mapper = config.CreateMapper();

            var request = new MapPriceRequest()
            {
                CustomerID = purchaseOrder.CustomerID,
                VendorID = purchaseOrder.VendorID,
                ShippingTermCode = purchaseOrder.ShippingTermCode,
                PurchaseOrderLines = purchaseOrder.PurchaseOrderLines
                                       .Select(x => mapper.Map<PurchaseOrderLineDto>(x)).ToList()
            };

            var response = service.MapPrice(request).Result;
            MessageOptions options = null;

            if (response != null)
            {
                if (response.Result.Code == "000")
                {
                    options = Message.GetMessageOptions(response.Result.Message, "Success",
                       InformationType.Success, null, 5000);

                    foreach (var line in purchaseOrder.PurchaseOrderLines)
                    {
                        var purchaseOrderLine = response.Data
                            .FirstOrDefault(x => x.ID == line.ID ||
                                                 (x.CustomerStyle == line.CustomerStyle &&
                                                  x.GarmentColorCode == line.GarmentColorCode &&
                                                  x.GarmentSize == line.GarmentSize &&
                                                  x.ItemID == line.ItemID &&
                                                  x.ItemName == line.ItemName &&
                                                  x.ItemColorCode == line.ItemColorCode &&
                                                  x.ItemColorName == line.ItemColorName &&
                                                  (x.Specify == line.Specify || string.IsNullOrEmpty(x.Specify))));

                        if (purchaseOrderLine != null)
                        {
                            line.Price = purchaseOrderLine.Price;
                        }
                    }

                    foreach (var groupLine in purchaseOrder.PurchaseOrderGroupLines)
                    {
                        var purchaseGroupOrderLine = response.Data
                            .FirstOrDefault(x => x.CustomerStyle == groupLine.CustomerStyle &&
                                                 x.GarmentColorCode == groupLine.GarmentColorCode &&
                                                 x.GarmentSize == groupLine.GarmentSize &&
                                                 x.ItemID == groupLine.ItemID &&
                                                 x.ItemName == groupLine.ItemName &&
                                                 x.ItemColorCode == groupLine.ItemColorCode &&
                                                 x.ItemColorName == groupLine.ItemColorName &&
                                                 (x.Specify == groupLine.Specify || string.IsNullOrEmpty(x.Specify)));

                        if (purchaseGroupOrderLine != null)
                        {
                            groupLine.Price = purchaseGroupOrderLine.Price;
                            groupLine.PurchaseOrderLines.ForEach(x =>
                            {
                                x.Price = purchaseGroupOrderLine.Price;
                            });
                        }
                    }
                }
                else
                {
                    options = Message.GetMessageOptions(response.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                options = Message.GetMessageOptions("Unknown error, contact your admin", "Error",
                   InformationType.Error, null, 5000);
            }

            View.Refresh();
            Application.ShowViewStrategy.ShowMessage(options);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
