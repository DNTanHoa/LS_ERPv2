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
    public partial class IssueContract_ViewController : ViewController
    {
        public IssueContract_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction issuedContractAction = new PopupWindowShowAction(this, "IssuedContractAction",
                PredefinedCategory.Unspecified);

            issuedContractAction.ImageName = "Header";
            issuedContractAction.Caption = "Contract";
            issuedContractAction.TargetObjectType = typeof(Issued);
            issuedContractAction.TargetViewType = ViewType.Any;
            issuedContractAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            issuedContractAction.Shortcut = "CtrlShiftP";

            issuedContractAction.CustomizePopupWindowParams += IssuedContractAction_CustomizePopupWindowParams;
            issuedContractAction.Execute += IssuedContractAction_Execute;
        }

        private void IssuedContractAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowViewCurrentObject as IssuedContractParam;

            MessageOptions message = null;
            var service = new IssuedService();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IssuedContractParam, CreateIssuedRequest>()
                    .ForMember(x => x.CustomerID, y => y.MapFrom(s => s.Customer.ID))
                    .ForMember(x => x.IssuedGroupLines, y => y.MapFrom(s => s.Details.Where(d => d.IssuedQuantity > 0)));
                cfg.CreateMap<Issued, UpdateIssuedRequest>();
                cfg.CreateMap<IssuedContractDetail, IssuedGroupLineDto>();
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
                        Roll = x.Roll
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

        private void IssuedContractAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = objectSpace.CreateObject<IssuedContractParam>();

            ///Init value
            model.IssuedBy = SecuritySystem.CurrentUserName;
            model.IssuedDate = DateTime.Now;
            model.ReceivedDate = DateTime.Now;
            model.MaterialTypeCode = "FB";
            model.Storage = objectSpace.GetObjects<Storage>().FirstOrDefault();

            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
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
