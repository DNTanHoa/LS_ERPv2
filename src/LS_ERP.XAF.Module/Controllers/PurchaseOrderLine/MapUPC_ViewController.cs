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
    public partial class MapUPC_ViewController : ViewController
    {
        public MapUPC_ViewController()
        {
            InitializeComponent();

            SimpleAction mapUPCAction = new SimpleAction(this, "MapUPC", PredefinedCategory.Unspecified);
            mapUPCAction.ImageName = "MergeAcross";
            mapUPCAction.Caption = "Map UPC";
            mapUPCAction.TargetObjectType = typeof(PurchaseOrderLine);
            mapUPCAction.TargetViewType = ViewType.ListView;
            mapUPCAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            mapUPCAction.Shortcut = "CtrlL";

            mapUPCAction.Execute += MapUPCAction_Execute;
        }

        private void MapUPCAction_Execute(object sender, SimpleActionExecuteEventArgs e)
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
            });

            var mapper = config.CreateMapper();

            var request = new MapUPCRequest()
            {
                CustomerID = purchaseOrder.CustomerID,
                PurchaseOrderLines = purchaseOrder.PurchaseOrderLines
                                        .Select(x => mapper.Map<PurchaseOrderLineDto>(x)).ToList()
            };

            var response = service.MapUPC(request).Result;
            MessageOptions options = null;

            if(response != null)
            {
                if(response.Result.Code == "000")
                {
                    options = Message.GetMessageOptions(response.Result.Message, "Success",
                       InformationType.Success, null, 5000);

                    if (response.Data.Any())
                    {
                        foreach(var line in purchaseOrder.PurchaseOrderLines)
                        {
                            var purchaseOrderLine = response.Data
                                .FirstOrDefault(x => x.ID == line.ID ||
                                                     (x.CustomerStyle == line.CustomerStyle &&
                                                      x.GarmentColorCode == line.GarmentColorCode &&
                                                      x.GarmentSize == line.GarmentSize));
                            if (purchaseOrderLine != null)
                            {
                                line.UPC = purchaseOrderLine.UPC;
                                line.Mfg = purchaseOrderLine.Mfg;
                                line.SuppPlt = purchaseOrderLine.SuppPlt;
                                line.ReplCode = purchaseOrderLine.ReplCode;
                                line.DeptSubFineline = purchaseOrderLine.DeptSubFineline;
                                line.FixtureCode = purchaseOrderLine.FixtureCode;
                                line.TagSticker = purchaseOrderLine.TagSticker;
                                line.ModelName = purchaseOrderLine.ModelName;
                                line.MSRP = purchaseOrderLine.MSRP;
                            }
                        }

                        foreach(var groupLine in purchaseOrder.PurchaseOrderGroupLines)
                        {
                            var purchaseOrderLine = response.Data
                                .FirstOrDefault(x => x.CustomerStyle == groupLine.CustomerStyle &&
                                                      x.GarmentColorCode == groupLine.GarmentColorCode &&
                                                      x.GarmentSize == groupLine.GarmentSize);
                            if(purchaseOrderLine != null)
                            {
                                groupLine.UPC = purchaseOrderLine.UPC;
                                groupLine.Mfg = purchaseOrderLine.Mfg;
                            }

                            groupLine.PurchaseOrderLines.ForEach(x =>
                            {
                                x.UPC = purchaseOrderLine.UPC;
                                x.Mfg = purchaseOrderLine.Mfg;
                                x.SuppPlt = purchaseOrderLine.SuppPlt;
                                x.ReplCode = purchaseOrderLine.ReplCode;
                                x.DeptSubFineline = purchaseOrderLine.DeptSubFineline;
                                x.FixtureCode = purchaseOrderLine.FixtureCode;
                                x.TagSticker = purchaseOrderLine.TagSticker;
                                x.ModelName = purchaseOrderLine.ModelName;
                                x.MSRP = purchaseOrderLine.MSRP;
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
