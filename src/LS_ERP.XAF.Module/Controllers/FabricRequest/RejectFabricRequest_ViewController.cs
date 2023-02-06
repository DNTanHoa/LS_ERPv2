using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class RejectFabricRequest_ViewController : ObjectViewController<ListView, FabricRequest>
    {
        public RejectFabricRequest_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction rejectFabricRequestAction = new PopupWindowShowAction(this, "RejectFabricRequest", PredefinedCategory.Unspecified);
            rejectFabricRequestAction.ImageName = "RemoveFooter";
            rejectFabricRequestAction.Caption = "Reject";
            rejectFabricRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //rejectFabricRequestAction.TargetObjectType = typeof(FabricRequest);
            //rejectFabricRequestAction.TargetViewType = ViewType.ListView;
            rejectFabricRequestAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            rejectFabricRequestAction.CustomizePopupWindowParams += RejectFabricRequestAction_CustomizePopupWindowParams;
            rejectFabricRequestAction.Execute += RejectFabricRequest_Execute;
        }

        public virtual void RejectFabricRequest_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

        }

        private void RejectFabricRequestAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var currentObject = View.CurrentObject as FabricRequest;

            //if (currentObject.StatusID != "A")
            //{
            List<FabricRequest> fabricRequests = new List<FabricRequest>();

            fabricRequests.Add(currentObject);

            var model = new SubmitFabricRequestParam()
            {
                FabricRequests = fabricRequests
            };

            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = false;
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
