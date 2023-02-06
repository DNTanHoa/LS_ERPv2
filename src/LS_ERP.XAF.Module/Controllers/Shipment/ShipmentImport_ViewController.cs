using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ShipmentImport_ViewController : ObjectViewController<ListView, Shipment>
    {
        public ShipmentImport_ViewController()
        {
            InitializeComponent();
            PopupWindowShowAction popupShipment = new PopupWindowShowAction(this,
                "PopupShipment", PredefinedCategory.Unspecified);
            popupShipment.ImageName = "Import";
            popupShipment.Caption = "Import";
            //popupShipment.TargetObjectType = typeof(Invoice);
            //popupShipment.TargetViewType = ViewType.DetailView;
            popupShipment.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //popupShipment.Shortcut = "CtrlShiftO";

            popupShipment.CustomizePopupWindowParams += PopupShipment_CustomizePopupWindowParams;
            popupShipment.Execute += PopupShipment_Execute;
        }

        private void PopupShipment_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowView.CurrentObject as ShipmentImportParam;
            var service = new ShipmentService();
            var invoice = e.CurrentObject as Invoice;
            var messageOptions = new MessageOptions();

            var request = new ShipmentImportRequest()
            {
                CustomerID = importParam.Customer?.ID,
                FilePath = importParam.ImportFilePath,
                UserName = SecuritySystem.CurrentUserName
            };

            var importResponse = service.Import(request).Result;

            if (importResponse != null)
            {
                if (importResponse.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions(importResponse.Result.Message, "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(importResponse.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
            }

            ObjectSpace.CommitChanges();
            View.Refresh();

            Application.ShowViewStrategy.ShowMessage(messageOptions);
        }

        private void PopupShipment_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            //var viewObject = View.CurrentObject as Invoice;

            var model = new ShipmentImportParam();
            model.Customer = objectSpace.GetObjects<Customer>().FirstOrDefault();

            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            //e.Maximized = true;
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
