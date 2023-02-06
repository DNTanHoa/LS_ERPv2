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
    public partial class LoadReceiptFromSearchParam_ViewController : ViewController
    {
        public LoadReceiptFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchReceiptAction = new SimpleAction(this, "SearchReceipt", PredefinedCategory.Unspecified);
            searchReceiptAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchReceiptAction.Caption = "Search (Ctrl + L)";
            searchReceiptAction.TargetObjectType = typeof(ReceiptSearchParam);
            searchReceiptAction.TargetViewType = ViewType.DetailView;
            searchReceiptAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchReceiptAction.Shortcut = "CtrlL";

            searchReceiptAction.Execute += SearchReceiptAction_Execute;
        }

        private void SearchReceiptAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as ReceiptSearchParam;

            if (searchParam != null)
            {
                var criteria = CriteriaOperator.Parse("(([Vendor] = ? OR ? Is Null) AND " +
                    "([ReceiptDate] <= ?) AND ([ReceiptDate] >= ?))",
                    searchParam.Vendor, searchParam.Vendor,
                    searchParam.ToDate, searchParam.FromDate);

                var objectSpace = Application.CreateObjectSpace(typeof(Receipt));
                var receipts = objectSpace.GetObjects<Receipt>(criteria);
                searchParam.Receipts = receipts.ToList();

                View.Refresh();
            }
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
