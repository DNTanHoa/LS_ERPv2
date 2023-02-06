using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ViewForecastMaterial_ViewController : ViewController
    {
        public ViewForecastMaterial_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction viewForecastMaterial = new PopupWindowShowAction(this, "ViewForecastMaterial", PredefinedCategory.Unspecified);
            viewForecastMaterial.ImageName = "SinglePageView";
            viewForecastMaterial.TargetObjectType = typeof(ForecastOverall);
            viewForecastMaterial.TargetViewType = ViewType.ListView;
            viewForecastMaterial.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            viewForecastMaterial.Caption = "Forecast Material";

            SimpleAction loadForecastMaterial = new SimpleAction(this, "LoadForecastMaterialFromViewParam", PredefinedCategory.Unspecified);
            loadForecastMaterial.ImageName = "RotateCounterclockwise";
            loadForecastMaterial.TargetObjectType = typeof(ViewForecastMaterialParam);
            loadForecastMaterial.TargetViewType = ViewType.DetailView;
            loadForecastMaterial.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadForecastMaterial.Caption = "Load (Ctrl + L)";
            loadForecastMaterial.Shortcut = "CtrlL";

            viewForecastMaterial.CustomizePopupWindowParams += ViewForecastMaterial_CustomizePopupWindowParams;
            viewForecastMaterial.Execute += ViewForecastMaterial_Execute;

            loadForecastMaterial.Execute += LoadForecastMaterial_Execute;
        }

        private void LoadForecastMaterial_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = e.CurrentObject as ViewForecastMaterialParam;

            var criteria = CriteriaOperator
                .Parse("ForecastOverallID = ?", viewObject.ForecastOverall.ID);
            viewObject.ForecastMaterials = ObjectSpace.GetObjects<ForecastMaterial>(criteria).ToList();

            View.Refresh();
        }

        private void ViewForecastMaterial_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void ViewForecastMaterial_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new ViewForecastMaterialParam()
            {
                ForecastOverall = View.CurrentObject as ForecastOverall
            };
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = Application.CreateDetailView(objectSpace, model, false);
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
