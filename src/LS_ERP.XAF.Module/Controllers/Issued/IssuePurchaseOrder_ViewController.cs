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
    public partial class IssuePurchaseOrder_ViewController : ViewController
    {
        public IssuePurchaseOrder_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction issuedPurchaseOrderAction = new PopupWindowShowAction(this, "IssuedPurchaseOrderAction",
                PredefinedCategory.Unspecified);

            issuedPurchaseOrderAction.ImageName = "Header";
            issuedPurchaseOrderAction.Caption = "Purchase";
            issuedPurchaseOrderAction.TargetObjectType = typeof(Issued);
            issuedPurchaseOrderAction.TargetViewType = ViewType.Any;
            issuedPurchaseOrderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            issuedPurchaseOrderAction.Shortcut = "CtrlShiftP";

            issuedPurchaseOrderAction.CustomizePopupWindowParams += IssuedPurchaseOrderAction_CustomizePopupWindowParams;
            issuedPurchaseOrderAction.Execute += IssuedPurchaseOrderAction_Execute;
        }

        private void IssuedPurchaseOrderAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowViewCurrentObject as IssuedPurchaseOrderParam;

            MessageOptions message = null;
            var service = new IssuedService();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IssuedPurchaseOrderParam, CreateIssuedRequest>()
                    .ForMember(x => x.CustomerID, y => y.MapFrom(s => s.PurchaseOrder.CustomerID))
                    .ForMember(x => x.IssuedGroupLines, y => y.MapFrom(s => s.Details));
                cfg.CreateMap<Issued, UpdateIssuedRequest>();
                cfg.CreateMap<IssuedPurchaseDetail, IssuedGroupLineDto>();
                cfg.CreateMap<IssuedLine, IssuedLineDto>();
            });

            var mapper = config.CreateMapper();

            var request = mapper.Map<CreateIssuedRequest>(viewObject);
            request.IssuedGroupLines.ForEach(x =>
            {
                x.IssuedLines = new List<IssuedLineDto>()
                {
                    new IssuedLineDto()
                    {
                        ItemID = x.ItemID,
                        ItemCode = x.ItemCode,
                        ItemName = x.ItemName,
                        ItemColorCode = x.ItemColorCode,
                        ItemColorName = x.ItemColorName,
                        Position = x.Position,
                        Specify = x.Specify,
                        Season = x.Season,
                        UnitID = x.UnitID,
                        CustomerStyle = x.CustomerStyle,
                        GarmentColorCode = x.GarmentColorCode,
                        GarmentColorName = x.GarmentColorName,
                        IssuedQuantity = x.IssuedQuantity,
                    }
                };
            });

            var response = service.CreateIssued(request).Result;

            if (response != null)
            {
                if (response.Result.Code == "000")
                {
                    message = Message.GetMessageOptions("Action successfully", "Success", InformationType.Success,
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

        private void IssuedPurchaseOrderAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            var model = new IssuedPurchaseOrderParam()
            {
                Storage = objectSpace.GetObjects<Storage>().FirstOrDefault(),
                ReceivedDate = DateTime.Now,
                IssuedDate = DateTime.Now,
                IssuedBy = SecuritySystem.CurrentUserName
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.View = view;
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
