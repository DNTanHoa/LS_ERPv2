using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Dtos;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class GetForecastMaterial_ViewController : ViewController
    {
        public GetForecastMaterial_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction getForecastMaterialAction = new PopupWindowShowAction(this, "GetForecastMaterialForPurchaseOrderLine",
               PredefinedCategory.Unspecified);

            getForecastMaterialAction.ImageName = "AutoSize_Fill";
            getForecastMaterialAction.Caption = "Forecast Material";
            getForecastMaterialAction.TargetObjectType = typeof(PurchaseOrderLine);
            getForecastMaterialAction.TargetViewType = ViewType.ListView;
            getForecastMaterialAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            getForecastMaterialAction.Shortcut = "CtrlShiftM";
            
            getForecastMaterialAction.CustomizePopupWindowParams += GetForecastMaterialAction_CustomizePopupWindowParams;
            getForecastMaterialAction.Execute += GetForecastMaterialAction_Execute;
        }

        private void GetForecastMaterialAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var purchaseOrder = (((ListView)View).CollectionSource as PropertyCollectionSource).MasterObject as PurchaseOrder;
            var objectSpace = this.ObjectSpace;
            var model = new PurchaseForecastMaterial()
            {
                VendorID = purchaseOrder.Vendor?.ID
            };
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = Application.CreateDetailView(objectSpace, model, false);
        }

        private void GetForecastMaterialAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var purchaseOrder = (((ListView)View).CollectionSource as PropertyCollectionSource).MasterObject
                as PurchaseOrder;

            ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
                .FindItem("ForecastMaterials") as ListPropertyEditor;
            MessageOptions message = null;

            var config = new MapperConfiguration(c =>
            {
                c.CreateMap<ForecastMaterial, ForecastMaterialDto>()
                    .ForMember(x => x.GarmentColorName, y => y.MapFrom(s => s.ForecastOverall.GarmentColorName))
                    .ForMember(x => x.GarmentColorCode, y => y.MapFrom(s => s.ForecastOverall.GarmentColorCode));

                c.CreateMap<ForecastOverall, ForecastOverallDto>();
                c.CreateMap<ForecastEntry, ForeCastEntryDto>();
                c.CreateMap<PurchaseOrderLine, PurchaseOrderLineDto>();
                c.CreateMap<PurchaseOrderGroupLine, PurchaseOrderGroupLineDto>();
                c.CreateMap<ReservationEntry, ReservationEntryDto>();
            });
            var mapper = config.CreateMapper();

            var service = new ForecastMaterialService();
            var request = new GroupForecastMaterialToPurchaseOrderLineRequest()
            {
                PurchaseOrderID = purchaseOrder.ID,
                PurchaseOrderLines = purchaseOrder.PurchaseOrderLines?
                                        .Where(x => x.ID > 0)
                                        .Select(x => mapper.Map<PurchaseOrderLineDto>(x)).ToList(),
                PurchaseOrderGroupLines = purchaseOrder.PurchaseOrderGroupLines?
                                        .Where(x => x.ID > 0)
                                        .Select(x => mapper.Map<PurchaseOrderGroupLineDto>(x)).ToList(),
                ForecastMaterials = listPropertyEditor.ListView.SelectedObjects.Cast<ForecastMaterial>()?
                                        .Select(x => mapper.Map<ForecastMaterialDto>(x)).ToList()
            };


            var response = service.GroupToPurchaseOrderLine(request).Result;

            if (response != null)
            {
                if (response.Result.Code == "000")
                {
                    List<PurchaseOrderGroupLine> groupLines = null;
                    List<PurchaseOrderLine> lines = null;

                    if (purchaseOrder.PurchaseOrderGroupLines == null)
                    {
                        purchaseOrder.PurchaseOrderGroupLines = new List<PurchaseOrderGroupLine>();
                        groupLines = new List<PurchaseOrderGroupLine>();
                    }
                    else
                    {
                        groupLines = purchaseOrder.PurchaseOrderGroupLines.ToList();
                    }

                    if (purchaseOrder.PurchaseOrderLines == null)
                    {
                        purchaseOrder.PurchaseOrderLines = new List<PurchaseOrderLine>();
                        lines = new List<PurchaseOrderLine>();
                    }
                    else
                    {
                        lines = purchaseOrder.PurchaseOrderLines.ToList();
                    }

                    /// Add new group line
                    groupLines.AddRange(response.Data?.PurchaseOrderGroupLines.Where(x => x.ID == 0));

                    /// Update old group line
                    foreach (var currentGroupLine in purchaseOrder.PurchaseOrderGroupLines)
                    {
                        if (currentGroupLine.ID > 0)
                        {
                            var groupLine = response.Data?.PurchaseOrderGroupLines
                                .FirstOrDefault(x => x.ID == currentGroupLine.ID);

                            if (groupLine != null)
                            {
                                currentGroupLine.Quantity = groupLine.Quantity;
                            }
                        }
                    }

                    lines.AddRange(response.Data?.PurchaseOrderLines.Where(x => x.ID == 0).ToList());

                    purchaseOrder.PurchaseOrderGroupLines
                            .AddRange(response.Data?.PurchaseOrderGroupLines.Where(x => x.ID == 0));
                    purchaseOrder.PurchaseOrderLines
                        = purchaseOrder.PurchaseOrderGroupLines.SelectMany(x => x.PurchaseOrderLines).ToList();
                }
                else
                {
                    message = Message.GetMessageOptions(response.Result.Message, "Error", InformationType.Error,
                        null, 5000);
                }
            }
            else
            {
                message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error", InformationType.Error,
                        null, 5000);
            }

            if (message != null)
                Application.ShowViewStrategy.ShowMessage(message);

            if (ObjectSpace is NonPersistentObjectSpace)
                ObjectSpace.Rollback(false);

            View.Refresh();
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
