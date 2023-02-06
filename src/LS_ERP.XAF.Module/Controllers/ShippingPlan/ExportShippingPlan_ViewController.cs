using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ExportShippingPlan_ViewController : ObjectViewController<DetailView, ShippingPlan>
    {
        public ExportShippingPlan_ViewController()
        {
            InitializeComponent();

            SimpleAction exportShippingPlan = new SimpleAction(this, "ExportShippingPlan", PredefinedCategory.Unspecified);
            exportShippingPlan.ImageName = "Export";
            exportShippingPlan.Caption = "Export";            
            exportShippingPlan.PaintStyle = ActionItemPaintStyle.CaptionAndImage;      
            exportShippingPlan.Execute += ExportShippingPlan_Execute;
        }

        public virtual void ExportShippingPlan_Execute(object sender, SimpleActionExecuteEventArgs e)
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
