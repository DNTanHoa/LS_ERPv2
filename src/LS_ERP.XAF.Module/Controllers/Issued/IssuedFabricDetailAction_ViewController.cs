using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class IssuedFabricDetailAction_ViewController : 
        ObjectViewController<ListView, IssuedFabric>
    {
        public IssuedFabricDetailAction_ViewController()
        {
            InitializeComponent();

            SimpleAction copyIssuedFabricQuantity = new SimpleAction(this, "CopyIssuedFabricQuantity", PredefinedCategory.Unspecified);
            copyIssuedFabricQuantity.ImageName = "Paste";
            copyIssuedFabricQuantity.Caption = "Copy Quantity";
            copyIssuedFabricQuantity.TargetObjectType = typeof(IssuedFabric);
            copyIssuedFabricQuantity.TargetViewType = ViewType.ListView;
            copyIssuedFabricQuantity.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            copyIssuedFabricQuantity.Shortcut = "CtrlL";

            copyIssuedFabricQuantity.Execute += CopyIssuedFabricQuantity_Execute;
        }

        private void CopyIssuedFabricQuantity_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObjects = View.SelectedObjects.Cast<IssuedFabric>();

            if(viewObjects != null &&
               viewObjects.Any())
            {
                foreach(var viewObject in viewObjects)
                {
                    viewObject.Roll = viewObject.RollInStock;
                    viewObject.IssuedQuantity = viewObject.InStockQuantity;
                }

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
