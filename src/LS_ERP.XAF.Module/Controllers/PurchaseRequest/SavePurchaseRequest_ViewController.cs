using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
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
    public partial class SavePurchaseRequest_ViewController : ViewController
    {
        public SavePurchaseRequest_ViewController()
        {
            InitializeComponent();

            SimpleAction savePurchaseRequestAction = new SimpleAction(this, "SavePurchaseRequest", PredefinedCategory.Unspecified);
            savePurchaseRequestAction.ImageName = "Save";
            savePurchaseRequestAction.Caption = "Save";
            savePurchaseRequestAction.TargetObjectType = typeof(PurchaseRequest);
            savePurchaseRequestAction.TargetViewType = ViewType.DetailView;
            savePurchaseRequestAction.PaintStyle = ActionItemPaintStyle.Default;
            savePurchaseRequestAction.Category = "Save";
            savePurchaseRequestAction.Shortcut = "CtrlS";

            savePurchaseRequestAction.Execute += SavePurchaseRequestAction_Execute;
        }

        private void SavePurchaseRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var purchaseRequest = View.CurrentObject as PurchaseRequest;
            MessageOptions message = null;
            var service = new PurchaseRequestService();

            foreach (var purchaseRequestLine in purchaseRequest?.PurchaseRequestGroupLines?
                .SelectMany(x => x.PurchaseRequestLines))
            {
                purchaseRequestLine.PurchaseRequestID = purchaseRequest.ID;
            }

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PurchaseRequest, SavePurchaseRequest>();
                cfg.CreateMap<PurchaseRequestLine, PurchaseRequestLineDto>();
                cfg.CreateMap<PurchaseRequestGroupLine, PurchaseRequestGroupLineDto>();
            });

            var mapper = config.CreateMapper();

            var request = mapper.Map<SavePurchaseRequest>(purchaseRequest);
            request.UserName = SecuritySystem.CurrentUserName;

            var response = service.SavePurchaseRequest(request).Result;

            if (response != null)
            {
                if (response.Result.Code == "000")
                {
                    message = Message.GetMessageOptions("Action successfully", "Success", 
                        InformationType.Success,
                        null, 5000);
                }
                else
                {
                    message = Message.GetMessageOptions(response.Result.Message, "Error", 
                        InformationType.Error,
                        null, 5000);
                }
            }
            else
            {
                message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error", 
                    InformationType.Error,
                        null, 5000);
            }

            if (message != null)
                Application.ShowViewStrategy.ShowMessage(message);

            View.Refresh();

            ModificationsController modificationsController = Frame.GetController<ModificationsController>();
            modificationsController.ModificationsHandlingMode = ModificationsHandlingMode.AutoRollback;
            ObjectSpace.Refresh();

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
