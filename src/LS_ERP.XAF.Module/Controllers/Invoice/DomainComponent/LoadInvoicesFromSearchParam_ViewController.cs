using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class LoadInvoicesFromSearchParam_ViewController : ViewController
    {
        public LoadInvoicesFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchInvoiceAction = new SimpleAction(this, "SearchInvoice", PredefinedCategory.Unspecified);
            searchInvoiceAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchInvoiceAction.Caption = "Search (Ctrl + L)";
            searchInvoiceAction.TargetObjectType = typeof(InvoiceSearchParam);
            searchInvoiceAction.TargetViewType = ViewType.DetailView;
            searchInvoiceAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchInvoiceAction.Shortcut = "CtrlL";

            searchInvoiceAction.Execute += SearchInvoiceAction_Execute;

            SimpleAction exportMultiInvoice = new SimpleAction(this, "ExportMultiInvoice", PredefinedCategory.Unspecified);
            exportMultiInvoice.ImageName = "Export";
            exportMultiInvoice.Caption = "Export (Ctrl + E)";
            exportMultiInvoice.TargetObjectType = typeof(InvoiceSearchParam);
            exportMultiInvoice.TargetViewType = ViewType.DetailView;
            exportMultiInvoice.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            exportMultiInvoice.Shortcut = "CtrlE";

            exportMultiInvoice.Execute += ExportMultiInvoice_Execute;
        }

        private void SearchInvoiceAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as InvoiceSearchParam;

            if (searchParam != null)
            {
                var criteria = CriteriaOperator.Parse("(([Customer] = ? OR ? Is Null) AND ([Date] <= ?) AND ([Date] >= ?))",
                    searchParam.Customer, searchParam.Customer, searchParam.OrderToDate, searchParam.OrderFromDate);

                var objectSpace = Application.CreateObjectSpace(typeof(Customer));
                var invoices = objectSpace.GetObjects<Invoice>(criteria);
                searchParam.Invoices = invoices.ToList();

                View.Refresh();
            }
        }
        public virtual void ExportMultiInvoice_Execute(object sender, SimpleActionExecuteEventArgs e)
        {

        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
