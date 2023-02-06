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
    public partial class GetProductionBomForPurchaseRequest_ViewController : ViewController
    {
        public GetProductionBomForPurchaseRequest_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction getProductionBomAction = new PopupWindowShowAction(this, "GetProductionBomForPurchaseRequestLine",
                PredefinedCategory.Unspecified);

            getProductionBomAction.ImageName = "Header";
            getProductionBomAction.Caption = "Production Bom";
            getProductionBomAction.TargetObjectType = typeof(PurchaseRequestLine);
            getProductionBomAction.TargetViewType = ViewType.ListView;
            getProductionBomAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            getProductionBomAction.Shortcut = "CtrlShiftB";

            getProductionBomAction.CustomizePopupWindowParams += GetProductionBomAction_CustomizePopupWindowParams;
            getProductionBomAction.Execute += GetProductionBomAction_Execute;
        }

        private void GetProductionBomAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var purchaseRequest = (((ListView)View).CollectionSource as PropertyCollectionSource).MasterObject
                 as PurchaseRequest;

            ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
                .FindItem("ProductionBOMs") as ListPropertyEditor;
            MessageOptions message = null;

            var config = new MapperConfiguration(c =>
            {
                c.CreateMap<ProductionBOM, ProductionBOMDto>()
                    .ForMember(x => x.GarmentColorName, y => y.MapFrom(s => s.ItemStyle.ColorName))
                    .ForMember(x => x.GarmentColorCode, y => y.MapFrom(s => s.ItemStyle.ColorCode));

                c.CreateMap<ItemStyle, ItemStyleDto>();
                c.CreateMap<PurchaseRequestLine, PurchaseRequestLineDto>();
                c.CreateMap<PurchaseRequestGroupLine, PurchaseRequestGroupLineDto>();
            });

            var mapper = config.CreateMapper();

            var service = new ProductionBOMService();
            var request = new GroupToPurchaseRequestLineRequest()
            {
                PurchaseRequestLines = purchaseRequest.PurchaseRequestLines?
                                        .Select(x => mapper.Map<PurchaseRequestLineDto>(x)).ToList(),
                PurchaseRequestGroupLines = purchaseRequest.PurchaseRequestGroupLines?
                                        .Select(x => mapper.Map<PurchaseRequestGroupLineDto>(x)).ToList(),
                ProductionBOMs = listPropertyEditor.ListView.SelectedObjects.Cast<ProductionBOM>()?
                                        .Select(x => mapper.Map<ProductionBOMDto>(x)).ToList()
            };

            var response = service.GroupToPurchaseRequestLine(request).Result;

            if (response != null)
            {
                if (response.Result.Code == "000")
                {
                    List<PurchaseRequestGroupLine> groupLines = null;
                    List<PurchaseRequestLine> lines = null;
                    if (purchaseRequest.PurchaseRequestGroupLines == null)
                    {
                        groupLines = new List<PurchaseRequestGroupLine>();
                    }
                    else
                    {
                        groupLines = purchaseRequest.PurchaseRequestGroupLines.ToList();
                    }

                    if (purchaseRequest.PurchaseRequestLines == null)
                    {
                        lines = new List<PurchaseRequestLine>();
                    }
                    else
                    {
                        lines = purchaseRequest.PurchaseRequestLines.ToList();
                    }

                    groupLines.AddRange(response.Data?.PurchaseRequestGroupLines);
                    lines.AddRange(response.Data?.PurchaseRequestLines);

                    purchaseRequest.PurchaseRequestLines = lines;
                    purchaseRequest.PurchaseRequestGroupLines = groupLines;
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

            View.Refresh();
            View.Refresh();
        }

        private void GetProductionBomAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new RequestProductionBOM();
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = Application.CreateDetailView(objectSpace, model, false);
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
