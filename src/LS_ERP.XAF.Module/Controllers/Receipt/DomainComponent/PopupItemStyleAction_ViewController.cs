using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class PopupItemStyleAction_ViewController 
        : ObjectViewController<DetailView, ItemStylePopupModel>
    {
        public PopupItemStyleAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchItemStyleForReceiptFinishGoodAction = new SimpleAction(this, "SearchItemStyleForReceiptFinishGood", PredefinedCategory.Unspecified);
            searchItemStyleForReceiptFinishGoodAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchItemStyleForReceiptFinishGoodAction.Caption = "Search (Ctrl + L)";
            searchItemStyleForReceiptFinishGoodAction.TargetObjectType = typeof(ItemStylePopupModel);
            searchItemStyleForReceiptFinishGoodAction.TargetViewType = ViewType.DetailView;
            searchItemStyleForReceiptFinishGoodAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchItemStyleForReceiptFinishGoodAction.Shortcut = "CtrlL";

            searchItemStyleForReceiptFinishGoodAction.Execute += SearchItemStyleForReceiptFinishGoodAction_Execute;

            SimpleAction loadOrderDetailForReceiptFinishGoodAction = new SimpleAction(this, "LoadOrderDetialForReceiptFinishGood", PredefinedCategory.Unspecified);
            loadOrderDetailForReceiptFinishGoodAction.ImageName = "RotateCounterclockwise";
            loadOrderDetailForReceiptFinishGoodAction.Caption = "Load Order (Shift + B)";
            loadOrderDetailForReceiptFinishGoodAction.TargetObjectType = typeof(ItemStylePopupModel);
            loadOrderDetailForReceiptFinishGoodAction.TargetViewType = ViewType.DetailView;
            loadOrderDetailForReceiptFinishGoodAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadOrderDetailForReceiptFinishGoodAction.Shortcut = "ShiftB";

            loadOrderDetailForReceiptFinishGoodAction.Execute += LoadOrderDetailForReceiptFinishGoodAction_Execute;

            SimpleAction copyOrderDetailForReceiptFinishGoodAction = new SimpleAction(this, "CopyOrderDetialForReceiptFinishGood", PredefinedCategory.Unspecified);
            copyOrderDetailForReceiptFinishGoodAction.ImageName = "TwoPageView";
            copyOrderDetailForReceiptFinishGoodAction.Caption = "Copy Quantity (Shift + B)";
            copyOrderDetailForReceiptFinishGoodAction.TargetObjectType = typeof(ItemStylePopupModel);
            copyOrderDetailForReceiptFinishGoodAction.TargetViewType = ViewType.DetailView;
            copyOrderDetailForReceiptFinishGoodAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            copyOrderDetailForReceiptFinishGoodAction.Shortcut = "ShiftC";

            copyOrderDetailForReceiptFinishGoodAction.Execute += CopyOrderDetailForReceiptFinishGoodAction_Execute;
        }

        private void CopyOrderDetailForReceiptFinishGoodAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var param = View.CurrentObject as ItemStylePopupModel;
            ListPropertyEditor listPropertyEditor = ((DetailView)View).FindItem("OrderDetailReceipt") 
                as ListPropertyEditor;

            if (listPropertyEditor != null)
            {
                var selectedOrderDetails = listPropertyEditor.ListView.SelectedObjects
                    .Cast<OrderDetailReceipt>().ToList();

                foreach(var orderDetail in selectedOrderDetails)
                {
                    orderDetail.ReceiptQuantity = orderDetail.OrderQuantity.ToString();
                }

                View.Refresh();
            }
        }

        private void LoadOrderDetailForReceiptFinishGoodAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var param = View.CurrentObject as ItemStylePopupModel;
            ListPropertyEditor listPropertyEditor = ((DetailView)View).FindItem("ItemStyles") as ListPropertyEditor;
            
            if(listPropertyEditor != null)
            {
                var selectedStyles = listPropertyEditor.ListView.SelectedObjects.Cast<ItemStyleReceipt>().ToList();
                var criteriaString = "Quantity > 0 AND " +
                        "ItemStyleNumber IN(" + string.Join(',', selectedStyles?.Select(x => "'" + x.Number + "'")) + ")";
                var criteria = CriteriaOperator.Parse(criteriaString);
                var orderDetails = ObjectSpace.GetObjects<OrderDetail>(criteria)
                    .Select(x => new OrderDetailReceipt()
                    {
                        CustomerStyle = x.ItemStyle.CustomerStyle,
                        GarmentColorCode = x.ItemStyle.ColorCode,
                        GarmentColorName = x.ItemStyle.ColorName,
                        LSSStyle = x.ItemStyle.LSStyle,
                        ProductDescription = x.ItemStyle.Description,
                        Season = x.ItemStyle.Season,
                        GarmentSize = x.Size,
                        OrderQuantity = (int?)x.Quantity ?? 0
                    });
                param.OrderDetailReceipt = orderDetails.ToList();
                View.Refresh();
            }
        }

        private void SearchItemStyleForReceiptFinishGoodAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var param = View.CurrentObject as ItemStylePopupModel;
            var criteria = CriteriaOperator.Parse("(Contains(LSStyle,?) OR ?) AND (Contains(PurchaseOrderNumber,?) OR ?)" +
                "AND SalesOrder.CustomerID = ?", param.Style,string.IsNullOrEmpty(param.Style), param.PurchaseOrderNumber, 
                string.IsNullOrEmpty(param.PurchaseOrderNumber), param.Customer.ID);
            
            var itemStyles = ObjectSpace.GetObjects<ItemStyle>(criteria)
                .Select(x => new ItemStyleReceipt()
                {
                    CustomerStyle = x.CustomerStyle,
                    GarmentColorCode = x.ColorCode,
                    GarmentColorName = x.ColorName,
                    LSSStyle = x.LSStyle,
                    ProductDescription = x.Description,
                    Number = x.Number,
                    Season = x.Season
                });
            
            param.ItemStyles = itemStyles.ToList();
            View.Refresh();

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
