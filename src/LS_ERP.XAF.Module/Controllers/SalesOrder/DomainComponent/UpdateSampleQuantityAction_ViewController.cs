using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class UpdateSampleQuantityAction_ViewController : ObjectViewController<DetailView, SalesOrderUpdateSampleQuantityParam>
    {
        public UpdateSampleQuantityAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserUpdateSampleQuantitySalesOrder = new SimpleAction(this,
                 "BrowserSalesOrderUpdateSimpleQuantityFile", PredefinedCategory.Unspecified);
            browserUpdateSampleQuantitySalesOrder.ImageName = "Open";
            browserUpdateSampleQuantitySalesOrder.Caption = "Browser";
            //browserUpdateShipQuantitySalesOrder.TargetObjectType = typeof(SalesOrderUpdateSimpleQuantityParam);
            //browserUpdateShipQuantitySalesOrder.TargetViewType = ViewType.DetailView;
            browserUpdateSampleQuantitySalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserUpdateSampleQuantitySalesOrder.Execute += BrowserUpdateSampleQuantitySalesOrder_Execute;
        }

        public virtual void BrowserUpdateSampleQuantitySalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
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
