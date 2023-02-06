using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ApprovePurchaseRequest_ViewController : ViewController
    {
        public ApprovePurchaseRequest_ViewController()
        {
            InitializeComponent();

            SimpleAction approvePurchaseRequestSubmit = new SimpleAction(this, 
                "ApprovePurchaseRequestSubmit", PredefinedCategory.Unspecified);
            approvePurchaseRequestSubmit.ImageName = "ApplyChanges";
            approvePurchaseRequestSubmit.Caption = "Approve";
            approvePurchaseRequestSubmit.TargetObjectType = typeof(PurchaseRequestLog);
            approvePurchaseRequestSubmit.TargetViewType = ViewType.ListView;
            approvePurchaseRequestSubmit.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            approvePurchaseRequestSubmit.TargetObjectsCriteria = "IsNullOrEmpty(ReferenceLogCode)";
            approvePurchaseRequestSubmit.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            approvePurchaseRequestSubmit.Execute += ApprovePurchaseRequestSubmit_Execute;
        }

        public virtual void ApprovePurchaseRequestSubmit_Execute(object sender, SimpleActionExecuteEventArgs e)
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
