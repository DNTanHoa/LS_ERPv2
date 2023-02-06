using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class UpdateSalesOrderAction_ViewController : ViewController
    {
        public UpdateSalesOrderAction_ViewController()
        {
            InitializeComponent();
            SimpleAction browserUpdateQuantitySalesOrder = new SimpleAction(this,
                "BrowserSalesOrderUpdateFile", PredefinedCategory.Unspecified);
            browserUpdateQuantitySalesOrder.ImageName = "Open";
            browserUpdateQuantitySalesOrder.Caption = "Browser";
            browserUpdateQuantitySalesOrder.TargetObjectType = typeof(SalesOrderUpdateParam);
            browserUpdateQuantitySalesOrder.TargetViewType = ViewType.DetailView;
            browserUpdateQuantitySalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserUpdateQuantitySalesOrder.Execute += BrowserUpdateSalesOrder_Execute;
        }

        public virtual void BrowserUpdateSalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
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
