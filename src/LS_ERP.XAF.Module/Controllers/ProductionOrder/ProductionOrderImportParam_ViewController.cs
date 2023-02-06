using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using System;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ProductionOrderImportParam_ViewController 
        : ObjectViewController<DetailView, ProductionOrderImportParam>
    {
        public ProductionOrderImportParam_ViewController()
        {
            InitializeComponent();

            SimpleAction browserImportProductionOrder = new SimpleAction(this, "BrowserImportProductionOrder", PredefinedCategory.Unspecified);
            browserImportProductionOrder.ImageName = "Open";
            browserImportProductionOrder.Caption = "Browser";
            browserImportProductionOrder.TargetObjectType = typeof(ProductionOrderImportParam);
            browserImportProductionOrder.TargetViewType = ViewType.DetailView;
            browserImportProductionOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            browserImportProductionOrder.Execute += BrowserImportProductionOrder_Execute;

            SimpleAction importProductionOrder = new SimpleAction(this, "ImportProductionOrderAction", PredefinedCategory.Unspecified);
            importProductionOrder.Caption = "Import";
            importProductionOrder.ImageName = "Import";
            importProductionOrder.TargetObjectType = typeof(ProductionOrderImportParam);
            importProductionOrder.TargetViewType = ViewType.DetailView;
            importProductionOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            importProductionOrder.Execute += ImportProductionOrder_Execute;
        }

        private void ImportProductionOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            throw new NotImplementedException();
        }

        public virtual void BrowserImportProductionOrder_Execute(object sender, SimpleActionExecuteEventArgs e)
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
