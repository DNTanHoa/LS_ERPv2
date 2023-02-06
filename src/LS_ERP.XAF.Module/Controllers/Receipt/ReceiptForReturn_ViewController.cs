using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ReceiptForReturn_ViewController 
        : ObjectViewController<DetailView, ReceiptSearchParam>
    {
        public ReceiptForReturn_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction receiptForReturn = new PopupWindowShowAction(this, "ReceiptForReturn",
                PredefinedCategory.Unspecified);

            receiptForReturn.ImageName = "TrackingChanges_TrackChanges";
            receiptForReturn.Caption = "Refund";
            receiptForReturn.TargetObjectType = typeof(ReceiptSearchParam);
            receiptForReturn.TargetViewType = ViewType.DetailView;
            receiptForReturn.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            receiptForReturn.Shortcut = "CtrlShiftB";

            receiptForReturn.CustomizePopupWindowParams += ReceiptForReturn_CustomizePopupWindowParams;
            receiptForReturn.Execute += ReceiptForReturn_Execute;
        }

        private void ReceiptForReturn_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowViewCurrentObject as ReturnPopupModel;
            MessageOptions message = null;

            if(viewObject != null)
            {
                var service = new ReceiptService();

                var request = new CreateReceiptRequest()
                {
                    ArrivedDate = viewObject.ReturnDate,
                    EntriedDate = viewObject.ReturnDate,
                    ReceiptDate = viewObject.ReturnDate,
                    ReceiptBy = SecuritySystem.CurrentUserName,
                    EntriedBy = SecuritySystem.CurrentUserName,
                    CustomerID = viewObject.Issued?.CustomerID,
                    StorageCode = viewObject.Storage?.Code,
                    ReceiptTypeId = "RR",
                    ReceiptGroupLines = viewObject.Details
                        .Where(x => x.ReturnQuantity > 0)
                        .Select(x => new ReceiptGroupLineRequest()
                        {
                            ItemName = x.ItemName,
                            ItemColorCode = x.ItemColorCode,
                            ItemMasterID = x.ItemMaterID,
                            ItemColorName = x.ItemColorName,
                            GarmentColorCode = x.GarmentColorCode,
                            GarmentColorName = x.GarmentColorName,
                            CustomerStyle = x.CustomerStyle,
                            LSStyle = x.LSStyle,
                            Season = x.Season,
                            Position = x.Position,
                            Specify = x.Specify,
                            ReceiptQuantity = x.ReturnQuantity
                        }).ToList(),
                };

                var respone = service.CreateReceipt(request).Result;

                if (respone != null)
                {
                    if (respone.Result.Code == "100")
                    {
                        message = Message.GetMessageOptions("Create successfully", "Success",
                            InformationType.Success, null, 5000);
                    }
                    else
                    {
                        message = Message.GetMessageOptions(respone.Result.Message, "Error",
                            InformationType.Error, null, 5000);
                    }
                }
                else
                {
                    message = Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
                }
            }
            else
            {
                message = Message.GetMessageOptions("Unexpected error", "Error", 
                    InformationType.Error, null, 2000);
            }

            if(message != null)
            {
                Application.ShowViewStrategy.ShowMessage(message);
            }
        }

        private void ReceiptForReturn_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            var model = objectSpace.CreateObject<ReturnPopupModel>();
            model.ReturnBy = SecuritySystem.CurrentUserName;
            model.ReturnDate = DateTime.Now;
            model.Storage = objectSpace.GetObjects<Storage>().FirstOrDefault();
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
