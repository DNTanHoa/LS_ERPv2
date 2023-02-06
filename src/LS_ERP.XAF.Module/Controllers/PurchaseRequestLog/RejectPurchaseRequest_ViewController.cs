using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class RejectPurchaseRequest_ViewController : ViewController
    {
        public RejectPurchaseRequest_ViewController()
        {
            InitializeComponent();

            SimpleAction rejectPurchaseRequestSubmit = new SimpleAction(this, 
                "RejectPurchaseRequestSubmit", PredefinedCategory.Unspecified);
            rejectPurchaseRequestSubmit.ImageName = "RemoveFooter";
            rejectPurchaseRequestSubmit.Caption = "Reject";
            rejectPurchaseRequestSubmit.TargetObjectType = typeof(PurchaseRequestLog);
            rejectPurchaseRequestSubmit.TargetViewType = ViewType.ListView;
            rejectPurchaseRequestSubmit.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            rejectPurchaseRequestSubmit.SelectionDependencyType = 
                SelectionDependencyType.RequireSingleObject;
            rejectPurchaseRequestSubmit.TargetObjectsCriteria = "IsNullOrEmpty(ReferenceLogCode)";

            rejectPurchaseRequestSubmit.Execute += RejectPurchaseRequestSubmit_Execute;
        }

        public virtual void RejectPurchaseRequestSubmit_Execute(object sender, 
            SimpleActionExecuteEventArgs e)
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
