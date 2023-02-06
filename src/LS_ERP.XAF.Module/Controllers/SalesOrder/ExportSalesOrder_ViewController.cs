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
    public partial class ExportSalesOrder_ViewController : ViewController
    {
        public ExportSalesOrder_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction exportSalesOrder = new PopupWindowShowAction(this, "ExportSalesOrder", PredefinedCategory.Unspecified);
            exportSalesOrder.Caption = "Export SO (Ctrl + E)";
            exportSalesOrder.ImageName = "Export";
            exportSalesOrder.TargetObjectType = typeof(SalesOrder);
            exportSalesOrder.TargetViewType = ViewType.ListView;
            exportSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            exportSalesOrder.Shortcut = "CtrlE";
            exportSalesOrder.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            exportSalesOrder.CustomizePopupWindowParams += ExportSalesOrder_CustomizePopupWindowParams;
            exportSalesOrder.Execute += ExportSalesOrder_Execute;
        }

        public virtual void ExportSalesOrder_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

        }

        void ExportSalesOrder_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var selected = View.SelectedObjects[0] as SalesOrder;

            var model = new SalesOrderExportParam()
            {
                Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(x => x.ID == selected.CustomerID),
                ShipDate = DateTime.Now
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
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
