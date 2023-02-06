using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ApprovedFabricRequest_ViewController : ObjectViewController<ListView, FabricRequest>
    {
        public ApprovedFabricRequest_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction submitFabricRequestAction = new PopupWindowShowAction(this, "ApprovedFabricRequest", PredefinedCategory.Unspecified);
            submitFabricRequestAction.ImageName = "Actions_Send";
            submitFabricRequestAction.Caption = "Approved";
            submitFabricRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //submitFabricRequestAction.TargetObjectType = typeof(FabricRequest);
            //submitFabricRequestAction.TargetViewType = ViewType.ListView;
            submitFabricRequestAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            submitFabricRequestAction.CustomizePopupWindowParams += SubmitFabricRequestAction_CustomizePopupWindowParams;
            submitFabricRequestAction.Execute += SubmitFabricRequest_Execute;

        }

        public virtual void SubmitFabricRequest_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

        }

        private void SubmitFabricRequestAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var currentObject = View.CurrentObject as FabricRequest;

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
