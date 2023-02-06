using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class UpdateCAC_CHD_EHDSalesOrderAction_ViewController : ObjectViewController<DetailView, SalesOrderUpdateCAC_CHD_EHDParam>
    {
        public UpdateCAC_CHD_EHDSalesOrderAction_ViewController()
        {
            InitializeComponent();

            SimpleAction browserUpdateCAC_CHD_EHDSalesOrder = new SimpleAction(this,
                            "BrowserSalesOrderUpdateCAC_CHD_EHDFile", PredefinedCategory.Unspecified);
            browserUpdateCAC_CHD_EHDSalesOrder.ImageName = "Open";
            browserUpdateCAC_CHD_EHDSalesOrder.Caption = "Browser";
            browserUpdateCAC_CHD_EHDSalesOrder.TargetObjectType = typeof(SalesOrderUpdateCAC_CHD_EHDParam);
            browserUpdateCAC_CHD_EHDSalesOrder.TargetViewType = ViewType.DetailView;
            browserUpdateCAC_CHD_EHDSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserUpdateCAC_CHD_EHDSalesOrder.Execute += BrowserUpdateCAC_CHD_EHDSalesOrder_Execute;
        }

        public virtual void BrowserUpdateCAC_CHD_EHDSalesOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
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
