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
    public partial class ViewProductionBom_ViewController : ViewController
    {
        public ViewProductionBom_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction viewProductionBom = new PopupWindowShowAction(this, "ViewProductionBomItemStyle", PredefinedCategory.Unspecified);
            viewProductionBom.ImageName = "SinglePageView";
            viewProductionBom.TargetObjectType = typeof(ItemStyle);
            viewProductionBom.TargetViewType = ViewType.ListView;
            viewProductionBom.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            viewProductionBom.Caption = "Production Bom";

            SimpleAction loadProductionBom = new SimpleAction(this, "LoadProductionFromViewParam", PredefinedCategory.Unspecified);
            loadProductionBom.ImageName = "RotateCounterclockwise";
            loadProductionBom.TargetObjectType = typeof(ViewProductionBomParam);
            loadProductionBom.TargetViewType = ViewType.DetailView;
            loadProductionBom.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadProductionBom.Caption = "Load (Ctrl + L)";
            loadProductionBom.Shortcut = "CtrlL";

            viewProductionBom.CustomizePopupWindowParams += ViewProductionBom_CustomizePopupWindowParams;
            viewProductionBom.Execute += ViewProductionBom_Execute;

            loadProductionBom.Execute += LoadProductionBom_Execute;
        }

        private void LoadProductionBom_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = e.CurrentObject as ViewProductionBomParam;

            var criteria = CriteriaOperator.Parse("ItemStyleNumber = ?", viewObject.ItemStyle.Number);
            viewObject.ProductionBOMs = ObjectSpace.GetObjects<ProductionBOM>(criteria).ToList();

            View.Refresh();
        }

        private void ViewProductionBom_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            
        }

        private void ViewProductionBom_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new ViewProductionBomParam()
            {
                ItemStyle = View.CurrentObject as ItemStyle,
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
