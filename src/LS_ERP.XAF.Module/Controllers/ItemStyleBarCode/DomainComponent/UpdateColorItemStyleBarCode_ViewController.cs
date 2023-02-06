using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class UpdateColorItemStyleBarCode_ViewController : ViewController
    {
        private IObjectSpace objectSpaceItemStyleBarCode;
        public UpdateColorItemStyleBarCode_ViewController()
        {
            InitializeComponent();

            SimpleAction updateColorItemStyleBarCodeAction = new SimpleAction(this, "UpdateColorItemStyleBarCodeAction", PredefinedCategory.Unspecified);
            updateColorItemStyleBarCodeAction.ImageName = "Save";
            updateColorItemStyleBarCodeAction.Caption = "Save (Ctrl+S)";
            updateColorItemStyleBarCodeAction.TargetObjectType = typeof(ItemStyleBarCode);
            updateColorItemStyleBarCodeAction.TargetViewType = ViewType.ListView;
            updateColorItemStyleBarCodeAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            updateColorItemStyleBarCodeAction.Shortcut = "CtrlS";
            updateColorItemStyleBarCodeAction.Execute += UpdateColorItemStyleBarCodeAction_Execute;
        }
        private void UpdateColorItemStyleBarCodeAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            objectSpaceItemStyleBarCode = Application.CreateObjectSpace(typeof(ItemStyleBarCode));
            var itemStyleBarCodes = ((ListView)View).CollectionSource.List
                                        .Cast<ItemStyleBarCode>().ToList();

            foreach (var data in itemStyleBarCodes)
            {
                var newItemStyleBarCode = objectSpaceItemStyleBarCode.GetObjectByKey<ItemStyleBarCode>(data.ID);
                newItemStyleBarCode.Color = data.Color;
            }

            objectSpaceItemStyleBarCode.CommitChanges();

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
