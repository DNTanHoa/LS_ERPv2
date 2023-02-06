using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Dtos;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class IssuedFabric_ViewController : ViewController
    {
        public IssuedFabric_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction issuedFabricAction = new PopupWindowShowAction(this, "IssuedFabricAction",
                PredefinedCategory.Unspecified);

            issuedFabricAction.ImageName = "Header";
            issuedFabricAction.Caption = "Fabric";
            issuedFabricAction.TargetObjectType = typeof(Issued);
            issuedFabricAction.TargetViewType = ViewType.Any;
            issuedFabricAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            issuedFabricAction.Shortcut = "CtrlShiftF";

            issuedFabricAction.CustomizePopupWindowParams += IssuedFabricAction_CustomizePopupWindowParams;
            issuedFabricAction.Execute += IssuedFabricAction_Execute;
        }

        private void IssuedFabricAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = objectSpace.CreateObject<IssuedFabricParam>();

            ///Init value
            model.IssuedBy = SecuritySystem.CurrentUserName;
            model.IssuedDate = DateTime.Now;
            model.ReceivedDate = DateTime.Now;
            model.Storage = objectSpace.GetObjects<Storage>().FirstOrDefault();

            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = view;
        }

        private void IssuedFabricAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;

            var viewObject = e.PopupWindowViewCurrentObject as IssuedFabricParam;

            MessageOptions message = null;
            var service = new IssuedService();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IssuedFabricParam, CreateIssuedRequest>()
                    .ForMember(x => x.CustomerID, y => y.MapFrom(s => s.Customer.ID))
                    .ForMember(x => x.IssuedGroupLines, y => y.MapFrom(s => s.Fabrics.Where(d => d.IssuedQuantity > 0)));
                cfg.CreateMap<IssuedFabric, IssuedGroupLineDto>();
                cfg.CreateMap<IssuedFabric, IssuedLineDto>();
            });

            var mapper = config.CreateMapper();

            var request = mapper.Map<CreateIssuedRequest>(viewObject);

            if (request.IssuedGroupLines == null)
            {
                request.IssuedGroupLines = new List<IssuedGroupLineDto>();
            }

            request.IssuedGroupLines.ForEach(x =>
            {
                x.IssuedLines = new List<IssuedLineDto>()
                {
                    new IssuedLineDto()
                    {
                        ItemID = x.ItemID,
                        DsmItemID = x.DsmItemID,
                        ItemCode = x.ItemCode,
                        ItemName = x.ItemName.Replace("\n",""),
                        ItemColorCode = x.ItemColorCode.Replace("\n",""),
                        ItemColorName = x.ItemColorName.Replace("\n",""),
                        Position = x.Position,
                        Specify = x.Specify,
                        Season = x.Season,
                        UnitID = x.UnitID,
                        CustomerStyle = x.CustomerStyle,
                        GarmentColorCode = x.GarmentColorCode,
                        GarmentColorName = x.GarmentColorName,
                        IssuedQuantity = x.IssuedQuantity,
                        Roll = x.Roll,
                        StorageDetailID = x.StorageDetailID,
                        LotNumber = x.LotNumber,
                        DyeLotNumber = x.DyeLotNumber,
                        FabricPurchaseOrderNumber = x.FabricPurchaseOrderNumber,
                        PurchaseOrderNumber = x.PurchaseOrderNumber,
                        FabricRequestDetailID = x.FabricRequestDetailID
                    }
                };
            });


            var response = service.CreateIssuedFabric(request).Result;

            if (response != null)
            {
                if (response.Result.Code == "000")
                {
                    message = Message.GetMessageOptions("Save successfully", "Success", InformationType.Success,
                        null, 5000);
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
