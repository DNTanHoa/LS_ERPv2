using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.Report.DomainComponent
{
    public partial class SalesOrderPurchaseReportAction_ViewController : ViewController
    {
        public SalesOrderPurchaseReportAction_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction viewMaterialForSalesOrder = new PopupWindowShowAction(this,
                "ViewMaterialForSalesOrder", PredefinedCategory.Unspecified);
            viewMaterialForSalesOrder.ImageName = "RepeatColumnHeadersOnEveryPage";
            viewMaterialForSalesOrder.Caption = "View Material";
            viewMaterialForSalesOrder.TargetObjectType = typeof(SalesOrderPurchaseReportDetail);
            viewMaterialForSalesOrder.TargetViewType = ViewType.ListView;
            viewMaterialForSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            viewMaterialForSalesOrder.Execute += ViewMaterialForSalesOrder_Execute;
            viewMaterialForSalesOrder.CustomizePopupWindowParams += ViewMaterialForSalesOrder_CustomizePopupWindowParams;
        }

        private void ViewMaterialForSalesOrder_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var styles = View.SelectedObjects.Cast<SalesOrderPurchaseReportDetail>();
            var model = new ViewItemStyleMaterialParam();
            var objectSpace = this.ObjectSpace;

            if (styles != null)
            {
                var service = new ItemStyleService();
                var response = service.GetMaterial(string.Join(",", styles.Select(x => x.LSStyle)))
                    .Result;
                if (response != null)
                {
                    model.Material = response.Data;
                }
            }

            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.View = view;
        }

        private void ViewMaterialForSalesOrder_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
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
