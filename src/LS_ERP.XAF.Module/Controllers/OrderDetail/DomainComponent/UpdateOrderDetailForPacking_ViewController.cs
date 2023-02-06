using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class UpdateOrderDetailForPacking_ViewController : ViewController
    {
        public UpdateOrderDetailForPacking_ViewController()
        {
            InitializeComponent();

            //SimpleAction updateOrderDetail = new SimpleAction(this, "UpdateOrderDetail", PredefinedCategory.Unspecified);
            //updateOrderDetail.ImageName = "Save";
            //updateOrderDetail.Caption = "Save (Ctrl+S)";
            //updateOrderDetail.TargetObjectType = typeof(OrderDetailForPacking);
            //updateOrderDetail.TargetViewType = ViewType.ListView;
            //updateOrderDetail.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //updateOrderDetail.Shortcut = "CtrlS";
            //updateOrderDetail.Execute += updateOrderDetail_Execute;
        }
        private void updateOrderDetail_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var gridData = ((ListView)View).CollectionSource.List;

            foreach (OrderDetailForPacking data in gridData)
            {
                var orderDetail = ObjectSpace.GetObjectByKey<OrderDetail>(data.OrderDetailID);
                orderDetail.ShipQuantity = data.ShipQuantity;
            }
            ObjectSpace.CommitChanges();

            View.Refresh();
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
