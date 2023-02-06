using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ReturnDetailModelAction_ViewController : ObjectViewController<ListView, ReturnDetailModel>
    {
        public ReturnDetailModelAction_ViewController()
        {
            InitializeComponent();

            SimpleAction copyReturnQuantity = new SimpleAction(this, "CopyReturnQuantity", 
                PredefinedCategory.Unspecified);
            copyReturnQuantity.ImageName = "Paste";
            copyReturnQuantity.Caption = "Copy Quantity";
            copyReturnQuantity.TargetObjectType = typeof(ReturnDetailModel);
            copyReturnQuantity.TargetViewType = ViewType.ListView;
            copyReturnQuantity.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            copyReturnQuantity.Execute += CopyReturnQuantity_Execute;
        }

        private void CopyReturnQuantity_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var returnDetails = View.SelectedObjects.Cast<ReturnDetailModel>();

            if (returnDetails != null &&
                returnDetails.Any())
            {
                foreach(var returnDetail in returnDetails)
                {
                    returnDetail.ReturnQuantity = returnDetail.IssuedQuantity;
                }

                View.Refresh();
            }
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
