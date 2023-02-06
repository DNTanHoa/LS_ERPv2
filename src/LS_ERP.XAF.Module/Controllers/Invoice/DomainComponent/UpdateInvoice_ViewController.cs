using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service.Invoice;
using LS_ERP.XAF.Module.Service.Request;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class UpdateInvoice_ViewController : ObjectViewController<DetailView, Invoice>
    {
        public UpdateInvoice_ViewController()
        {
            InitializeComponent();

            SimpleAction updateInvoice = new SimpleAction(this, "UpdateInvoice", PredefinedCategory.Unspecified);
            updateInvoice.ImageName = "Update";
            updateInvoice.Caption = "Update";
            updateInvoice.TargetObjectType = typeof(Invoice);
            updateInvoice.TargetViewType = ViewType.DetailView;
            updateInvoice.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //exportSalesContract.Shortcut = "CtrlShiftP";
            updateInvoice.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            updateInvoice.Execute += UpdateInvoice_Execute;
        }

        public void UpdateInvoice_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var selected = View.CurrentObject as Invoice;
            var service = new InvoiceService();
            var messageOptions = new MessageOptions();

            var request = new UpdateInvoiceRequest()
            {

                UserName = SecuritySystem.CurrentUserName,
                InvoiceID = selected.ID.ToString(),
                CustomerID = selected.CustomerID

            };
            var updateResponse = service.UpdateInvoice(request).Result;

            if (updateResponse != null)
            {
                if (updateResponse.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions(updateResponse.Result.Message, "Success",
                        InformationType.Success, null, 5000);
                    selected = updateResponse.Data;
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(updateResponse.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
            }

            ObjectSpace.CommitChanges();
            Application.ShowViewStrategy.ShowMessage(messageOptions);

            //View.RefreshDataSource();
            View.ObjectSpace.Refresh();
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();

            //if (View.Id == "Invoice_DetailView")
            //{

            //}
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
