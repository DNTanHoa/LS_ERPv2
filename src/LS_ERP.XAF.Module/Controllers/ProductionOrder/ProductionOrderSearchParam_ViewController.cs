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
    public partial class ProductionOrderSearchParam_ViewController : 
        ObjectViewController<DetailView, ProductionOrderSearchParam>
    {
        public ProductionOrderSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchProductionOrderAction = new SimpleAction(this, "SearchProductionOrderAction", PredefinedCategory.Unspecified);
            searchProductionOrderAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchProductionOrderAction.Caption = "Search (Ctrl + L)";
            searchProductionOrderAction.TargetObjectType = typeof(ProductionOrderSearchParam);
            searchProductionOrderAction.TargetViewType = ViewType.DetailView;
            searchProductionOrderAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchProductionOrderAction.Shortcut = "CtrlL";

            searchProductionOrderAction.Execute += SearchProductionOrderAction_Execute;

            PopupWindowShowAction importProductionOrder = new PopupWindowShowAction(this, "ImportProductionOrder", PredefinedCategory.Unspecified);
            importProductionOrder.ImageName = "Action_Search_Object_FindObjectByID";
            importProductionOrder.Caption = "Import";
            importProductionOrder.TargetObjectType = typeof(ProductionOrderSearchParam);
            importProductionOrder.TargetViewType = ViewType.DetailView;
            importProductionOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            importProductionOrder.Shortcut = "CtrlL";

            importProductionOrder.Execute += ImportProductionOrder_Execute;
            importProductionOrder.CustomizePopupWindowParams += ImportProductionOrder_CustomizePopupWindowParams;
        }

        private void ImportProductionOrder_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace(typeof(ProductionOrder));
            var param = new ProductionOrderImportParam()
            {
                Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(),
                StartDate = DateTime.Now
            };
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            var view = Application.CreateDetailView(objectSpace, param, false);
            e.View = view;
        }

        private void ImportProductionOrder_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SearchProductionOrderAction_Execute(object sender, SimpleActionExecuteEventArgs e)
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
