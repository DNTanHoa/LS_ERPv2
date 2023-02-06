using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service.SalesContract;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class DeleteSalesContract_ViewController : ViewController
    {
        public DeleteSalesContract_ViewController()
        {
            InitializeComponent();

            SimpleAction deleteSalesContract = new SimpleAction(this, "DeleteSalesContract", PredefinedCategory.Unspecified);
            deleteSalesContract.ImageName = "Delete";
            deleteSalesContract.Caption = "Delete (Ctrl + D)";
            deleteSalesContract.TargetObjectType = typeof(SalesContract);
            deleteSalesContract.TargetViewNesting = Nesting.Nested;
            deleteSalesContract.TargetViewType = ViewType.ListView;
            deleteSalesContract.PaintStyle = ActionItemPaintStyle.Image;
            deleteSalesContract.ConfirmationMessage = "Do you want to delete sales contract?";
            deleteSalesContract.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            deleteSalesContract.Execute += DeleteSalesContract_Execute;
        }

        private void DeleteSalesContract_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var contract = View.CurrentObject as SalesContract;
            MessageOptions message = null;

            if (contract != null)
            {
                var service = new SalesContractService();
                var response = service.DeleteSalesContract(contract.ID).Result;

                if (response != null)
                {
                    if (response.Result.Code == "000")
                    {
                        message = Message.GetMessageOptions(response.Result.Message, "Success",
                           InformationType.Success, null, 5000);
                    }
                    else
                    {
                        message = Message.GetMessageOptions(response.Result.Message, "Error",
                            InformationType.Error, null, 5000);
                    }
                }
                else
                {
                    message = Message.GetMessageOptions("Unknown error. Please contact admin", "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                message = Message.GetMessageOptions("Unknown error. Please contact admin", "Error",
                    InformationType.Error, null, 5000);
            }

            Application.ShowViewStrategy.ShowMessage(message);
            View.Refresh();
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
