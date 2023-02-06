using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class CancelSubmit_ViewController : ViewController
    {
        public CancelSubmit_ViewController()
        {
            InitializeComponent();

            SimpleAction cancelBomSubmit = new SimpleAction(this, "CancelBomSubmit", PredefinedCategory.Unspecified);
            cancelBomSubmit.ImageName = "Action_Cancel";
            cancelBomSubmit.Caption = "Cancel";
            cancelBomSubmit.TargetObjectType = typeof(PartRevisionLog);
            cancelBomSubmit.TargetViewType = ViewType.ListView;
            cancelBomSubmit.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            cancelBomSubmit.TargetObjectsCriteria = "IsNullOrEmpty(PartRevisionLogReferenceCode)";

            cancelBomSubmit.Execute += CancelBomSubmit_Execute; ;
        }

        public virtual void CancelBomSubmit_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            
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
