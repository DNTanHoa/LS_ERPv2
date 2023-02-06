using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ChooseNotify_ViewController : ViewController
    {
        public ChooseNotify_ViewController()
        {
            InitializeComponent();
            PopupWindowShowAction popupNotify = new PopupWindowShowAction(this,
                "PopupNotify", PredefinedCategory.Unspecified);
            popupNotify.ImageName = "Header";
            popupNotify.Caption = "Choose Notify";
            popupNotify.TargetObjectType = typeof(Invoice);
            popupNotify.TargetViewType = ViewType.DetailView;
            popupNotify.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            popupNotify.CustomizePopupWindowParams += PopupNotify_CustomizePopupWindowParams;
            popupNotify.Execute += PopupNotify_Execute;
        }

        private void PopupNotify_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            //var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as Invoice;
            ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
                .FindItem("Consignee") as ListPropertyEditor;

            var consignees = listPropertyEditor.ListView?.SelectedObjects.Cast<Consignee>();

            if (viewObject.NotifyParties == null)
            {
                viewObject.NotifyParties = new List<Consignee>();
            }

            foreach (var consignee in consignees)
            {
                viewObject.NotifyParties.Add(consignee);
            }

            View.Refresh();
        }

        private void PopupNotify_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as Invoice;

            var criteria = CriteriaOperator.Parse("[CustomerID] = ? ", viewObject.Customer?.ID);

            var model = new ChooseNotifyParam();
            model.Consignee = objectSpace.GetObjects<Consignee>(criteria).ToList();

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
