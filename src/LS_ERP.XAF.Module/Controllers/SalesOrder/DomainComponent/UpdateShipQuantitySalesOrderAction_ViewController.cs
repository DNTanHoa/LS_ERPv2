using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class UpdateShipQuantitySalesOrderAction_ViewController : ObjectViewController<DetailView, SalesOrderUpdateShipQuantityParam>
    {
        public UpdateShipQuantitySalesOrderAction_ViewController()
        {
            InitializeComponent(); 
            
            SimpleAction browserUpdateShipQuantitySalesOrder = new SimpleAction(this,
                 "BrowserSalesOrderUpdateShipQuantityFile", PredefinedCategory.Unspecified);
            browserUpdateShipQuantitySalesOrder.ImageName = "Open";
            browserUpdateShipQuantitySalesOrder.Caption = "Browser";
            browserUpdateShipQuantitySalesOrder.TargetObjectType = typeof(SalesOrderUpdateShipQuantityParam);
            browserUpdateShipQuantitySalesOrder.TargetViewType = ViewType.DetailView;
            browserUpdateShipQuantitySalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserUpdateShipQuantitySalesOrder.Execute += BrowserUpdateShipQuantitySalesOrder_Execute;
        }

        public virtual void BrowserUpdateShipQuantitySalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
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
