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
    public partial class LoadItemPriceFromSearchParam_ViewController : ViewController
    {
        public LoadItemPriceFromSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction searchItemPriceAction = new SimpleAction(this, "SearchItemPrice", PredefinedCategory.Unspecified);
            searchItemPriceAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchItemPriceAction.Caption = "Search (Ctrl + L)";
            searchItemPriceAction.TargetObjectType = typeof(ItemPriceSearchParam);
            searchItemPriceAction.TargetViewType = ViewType.DetailView;
            searchItemPriceAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchItemPriceAction.Shortcut = "CtrlL";

            SimpleAction newItemPriceAction = new SimpleAction(this, "NewItemPrice", PredefinedCategory.Unspecified);
            newItemPriceAction.ImageName = "Action_New";
            newItemPriceAction.Caption = "New Price";
            newItemPriceAction.TargetObjectType = typeof(ItemPriceSearchParam);
            newItemPriceAction.TargetViewType = ViewType.DetailView;
            newItemPriceAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchItemPriceAction.Execute += SearchItemPriceAction_Execute;
            newItemPriceAction.Execute += NewItemPriceAction_Execute;
        }

        private void NewItemPriceAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(PurchaseOrder));
            var model = new PurchaseOrder();
            e.ShowViewParameters.CreatedView = Application.CreateDetailView(objectSpace, model, true);
        }

        private void SearchItemPriceAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var searchParam = View.CurrentObject as ItemPriceSearchParam;

            if (searchParam != null)
            {
                var criteria = CriteriaOperator.Parse("((Contains([Vendor],?) OR ? Is Null) AND ([EffectDate] <= ?) AND ([EffectDate] >= ?))",
                    searchParam.Vendor, searchParam.Vendor, searchParam.EffectTo, searchParam.EffectFrom);

                var objectSpace = Application.CreateObjectSpace(typeof(ItemPrice));
                var itemPrices = objectSpace.GetObjects<ItemPrice>(criteria);
                searchParam.ItemPrices = itemPrices.ToList();

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
