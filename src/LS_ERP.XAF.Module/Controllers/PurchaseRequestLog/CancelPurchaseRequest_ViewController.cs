using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class CancelPurchaseRequest_ViewController : ViewController
    {
        public CancelPurchaseRequest_ViewController()
        {
            InitializeComponent();

            SimpleAction cancelPurchaseRequestSubmit = new SimpleAction(this, "CancelPurchaseRequestSubmit", PredefinedCategory.Unspecified);
            cancelPurchaseRequestSubmit.ImageName = "Action_Cancel";
            cancelPurchaseRequestSubmit.Caption = "Cancel";
            cancelPurchaseRequestSubmit.TargetObjectType = typeof(PurchaseRequestLog);
            cancelPurchaseRequestSubmit.TargetViewType = ViewType.ListView;
            cancelPurchaseRequestSubmit.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            cancelPurchaseRequestSubmit.TargetObjectsCriteria = "IsNullOrEmpty(ReferenceLogCode)";

            cancelPurchaseRequestSubmit.Execute += CancelPurchaseRequestSubmit_Execute;
        }

        private void CancelPurchaseRequestSubmit_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

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
