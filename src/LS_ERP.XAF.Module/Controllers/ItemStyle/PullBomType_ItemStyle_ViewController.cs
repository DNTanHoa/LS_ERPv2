using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class PullBomType_ItemStyle_ViewController : ViewController
    {
        public PullBomType_ItemStyle_ViewController()
        {
            InitializeComponent();
            PopupWindowShowAction importPurchaseOrder = new PopupWindowShowAction(this, "PullBomTypeItemStyle", PredefinedCategory.Unspecified);
            importPurchaseOrder.ImageName = "BringToFront"; //
            importPurchaseOrder.Caption = "Pull Bom Type"; //
            importPurchaseOrder.TargetObjectType = typeof(ItemStyle);
            importPurchaseOrder.TargetViewType = ViewType.ListView;
            importPurchaseOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;


            importPurchaseOrder.CustomizePopupWindowParams += PullBomPopup_CustomizePopupWindowParams;
            importPurchaseOrder.Execute += PullBomAction_Execute;
        }

        private void PullBomAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var itemStyleBomPulled = e.SelectedObjects.Cast<ItemStyle>().FirstOrDefault(x => x.IsBomPulled == true);

            if (itemStyleBomPulled != null)
            {
                var messageOptions = Message.GetMessageOptions("Your select has style which bom is pulled. Do you want to continue?",
                    "Warning", InformationType.Warning, new Action(() => PullBom(e)), 500);
                Application.ShowViewStrategy.ShowMessage(messageOptions);
            }
            else
            {
                PullBom(e);
            }
        }

        private void PullBom(PopupWindowShowActionExecuteEventArgs e)
        {
            PullBomParam selectType = e.PopupWindowViewCurrentObject as PullBomParam;
            var itemStyles = e.SelectedObjects.Cast<ItemStyle>();
            var itemStyleNumbers = itemStyles.Select(x => x.Number);
            var customerID = itemStyles.FirstOrDefault().SalesOrder?.CustomerID;
            var salesOrderID = itemStyles.FirstOrDefault().SalesOrder?.ID;
            var season = itemStyles.FirstOrDefault().Season;
            MessageOptions messageOptions = null;

            var service = new ItemStyleService();

            CommonRespone respone;

            if (selectType.PullBomTypes.Code.Equals("PA"))
            {
                var request = new PullBomItemStyleRequest()
                {
                    CustomerID = customerID,
                    UserName = SecuritySystem.CurrentUserName,
                    ItemStyleNumbers = itemStyleNumbers.ToList(),
                };

                respone = service.PullBom(request).Result;
            }
            else
            {
                var request = new PullBomTypeItemStyleRequest()
                {
                    CustomerID = customerID,
                    PullBomTypeCode = selectType.PullBomTypes.Code,
                    UserName = SecuritySystem.CurrentUserName,
                    ItemStyleNumbers = itemStyleNumbers.ToList(),
                    Season = season.Replace(" ", "").Trim(),
                    SalesOrderID = salesOrderID

                };

                respone = service.PullBomType(request).Result;
            }

            if (respone != null)
            {
                if (respone.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions("Pull successfully", "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(respone.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Unexpected error", "Error",
                    InformationType.Error, null, 5000);
            }

            Application.ShowViewStrategy.ShowMessage(messageOptions);

            View.Refresh();
        }

        void PullBomPopup_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new PullBomParam();
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.View = view;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
