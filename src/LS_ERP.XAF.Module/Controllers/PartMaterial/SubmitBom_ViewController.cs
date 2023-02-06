using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class SubmitBom_ViewController : ViewController
    {
        public SubmitBom_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction submitBomAction = new PopupWindowShowAction(this, "SubmitBom", PredefinedCategory.Unspecified);
            submitBomAction.ImageName = "Actions_Send";
            submitBomAction.Caption = "Submit";
            submitBomAction.TargetObjectType = typeof(PartMaterial);
            submitBomAction.TargetViewType = ViewType.ListView;
            submitBomAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            submitBomAction.CustomizePopupWindowParams += SubmitBomAction_CustomizePopupWindowParams;
            submitBomAction.Execute += SubmitBomAction_Execute;
            
        }

        public virtual void SubmitBomAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            View.Refresh();
        }

        private void SubmitBomAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new SubmitBomParam()
            {
                PartMaterials = View.SelectedObjects.Cast<PartMaterial>()
                                .Where(x => x.PartMaterialStatus?.Name != "Submitted" &&
                                            x.PartMaterialStatus?.Name != "Approved").ToList(),
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = view;
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
