using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class GetRemainQuantity_ViewController : ObjectViewController<ListView, PackingOverQuantity>
    {
        public GetRemainQuantity_ViewController()
        {
            InitializeComponent();

            SimpleAction getRemainQuantity = new SimpleAction(this, "GetRemainQuantityAction", PredefinedCategory.Unspecified);
            getRemainQuantity.ImageName = "TopDown";
            getRemainQuantity.Caption = "Get Remain";
            getRemainQuantity.TargetObjectType = typeof(PackingOverQuantity);
            getRemainQuantity.TargetViewType = ViewType.ListView;
            getRemainQuantity.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            getRemainQuantity.Execute += GetRemainQuantity_Execute;
        }

        private void GetRemainQuantity_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            PropertyCollectionSource collectionSource = (View as ListView).CollectionSource as PropertyCollectionSource;
            var itemStyle = collectionSource.MasterObject as ItemStyle;

            var objectSpace = Application.CreateObjectSpace(typeof(PackingOverQuantity));

            if (!itemStyle.PackingOverQuantities.Any())
            {
                foreach(var orderDetail in itemStyle.OrderDetails)
                {
                    var packingOverQuantity = objectSpace.CreateObject<PackingOverQuantity>();
                    packingOverQuantity.ItemStyleNumber = itemStyle.Number;
                    packingOverQuantity.ColorCode = itemStyle.ColorCode;
                    packingOverQuantity.ColorName = itemStyle.ColorName;
                    packingOverQuantity.Size = orderDetail.Size;
                    packingOverQuantity.SizeSortIndex = orderDetail.SizeSortIndex;
                    packingOverQuantity.Quantity = orderDetail.Quantity;
                    packingOverQuantity.SetCreateAudit(SecuritySystem.CurrentUserName);

                    itemStyle.PackingOverQuantities.Add(packingOverQuantity);
                }

                objectSpace.CommitChanges();
                //ObjectSpace.Refresh();
                View.Refresh(true);
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
