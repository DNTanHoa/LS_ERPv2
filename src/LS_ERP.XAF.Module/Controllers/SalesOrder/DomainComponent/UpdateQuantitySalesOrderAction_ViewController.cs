using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class UpdateQuantitySalesOrderAction_ViewController : ObjectViewController<DetailView, SalesOrderUpdateQuantityParam>
    {
        public UpdateQuantitySalesOrderAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserUpdateQuantitySalesOrder = new SimpleAction(this, 
                "BrowserSalesOrderUpdateQuantityFile", PredefinedCategory.Unspecified);
            browserUpdateQuantitySalesOrder.ImageName = "Open";
            browserUpdateQuantitySalesOrder.Caption = "Browser";
            browserUpdateQuantitySalesOrder.TargetObjectType = typeof(SalesOrderUpdateQuantityParam);
            browserUpdateQuantitySalesOrder.TargetViewType = ViewType.DetailView;
            browserUpdateQuantitySalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserUpdateQuantitySalesOrder.Execute += BrowserUpdateQuantitySalesOrder_Execute;
        }

        public virtual void BrowserUpdateQuantitySalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
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
