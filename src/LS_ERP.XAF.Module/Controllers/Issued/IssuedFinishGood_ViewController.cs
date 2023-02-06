using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class IssuedFinishGood_ViewController : ViewController
    {
        public IssuedFinishGood_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction issuedFinishGoodAction = new PopupWindowShowAction(this, "IssuedFinishGoodAction",
                PredefinedCategory.Unspecified);

            issuedFinishGoodAction.ImageName = "Header";
            issuedFinishGoodAction.Caption = "Finish Good";
            issuedFinishGoodAction.TargetObjectType = typeof(Issued);
            issuedFinishGoodAction.TargetViewType = ViewType.Any;
            issuedFinishGoodAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            issuedFinishGoodAction.Shortcut = "CtrlShiftG";

            issuedFinishGoodAction.CustomizePopupWindowParams += IssuedFinishGoodAction_CustomizePopupWindowParams;
            issuedFinishGoodAction.Execute += IssuedFinishGoodAction_Execute;
        }

        private void IssuedFinishGoodAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = objectSpace.CreateObject<IssuedFinishGoodParam>();

            ///Init value
            model.IssuedDate = DateTime.Now;
            model.Storage = objectSpace.GetObjects<Storage>().FirstOrDefault();

            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = view;
        }

        private void IssuedFinishGoodAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
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
