using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class StorageBinEntryInput_ViewController : ViewController
    {
        public StorageBinEntryInput_ViewController()
        {
            InitializeComponent();

            SimpleAction searchFinishToEntryBin = new SimpleAction(this, "SearchFinishToEntryBin", PredefinedCategory.Unspecified);
            searchFinishToEntryBin.ImageName = "Action_Search_Object_FindObjectByID";
            searchFinishToEntryBin.Caption = "Search (Ctrl + L)";
            searchFinishToEntryBin.TargetObjectType = typeof(FinishGoodBinEntryParam);
            searchFinishToEntryBin.TargetViewType = ViewType.DetailView;
            searchFinishToEntryBin.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            
            searchFinishToEntryBin.Execute += SearchFinishToEntryBin_Execute;
        }

        private void SearchFinishToEntryBin_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as FinishGoodBinEntryParam;

            if(viewObject != null)
            {
                var criteria = CriteriaOperator.Parse("(Contains(LSStyle,?) OR ?) AND (Contains(PurchaseOrderNumber,?) OR ?) AND ([SalesOrder.CustomerID] = ? OR ?)",
                    viewObject.Style, string.IsNullOrEmpty(viewObject.Style), viewObject.PurchaseOrderNumber, 
                    string.IsNullOrEmpty(viewObject.PurchaseOrderNumber),
                viewObject.Customer?.ID, string.IsNullOrEmpty(viewObject.Customer?.ID));
                var itemStyles = ObjectSpace.GetObjects<ItemStyle>(criteria)
                    .Select(x => new StyleFinishGoodBinEntry()
                    {
                        CustomerStyle = x.CustomerStyle,
                        LSStyle = x.LSStyle,
                        PurchaseOrderNumber = x.PurchaseOrderNumber,
                        GarmentColorCode = x.ColorCode,
                        GarmentColorName = x.ColorName,
                        Season = x.Season
                    });
                viewObject.Entries = itemStyles.ToList();
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
