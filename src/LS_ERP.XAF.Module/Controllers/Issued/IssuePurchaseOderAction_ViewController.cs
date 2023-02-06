using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class IssuePurchaseOderAction_ViewController : ViewController
    {
        public IssuePurchaseOderAction_ViewController()
        {
            InitializeComponent();

            SimpleAction loadPurchaseToIssuedAction = new SimpleAction(this, "LoadPurchaseToIssued", PredefinedCategory.Unspecified);
            loadPurchaseToIssuedAction.ImageName = "Action_Search_Object_FindObjectByID";
            loadPurchaseToIssuedAction.Caption = "Search (Ctrl + L)";
            loadPurchaseToIssuedAction.TargetObjectType = typeof(IssuedPurchaseOrderParam);
            loadPurchaseToIssuedAction.TargetViewType = ViewType.DetailView;
            loadPurchaseToIssuedAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadPurchaseToIssuedAction.Shortcut = "CtrlL";

            loadPurchaseToIssuedAction.Execute += LoadPurchaseToIssuedAction_Execute;

            SimpleAction copyQuantityPurchaseToIssuedAction = new SimpleAction(this, "CopyQuantityPurchaseToIssuedAction", PredefinedCategory.Unspecified);
            copyQuantityPurchaseToIssuedAction.ImageName = "Action_Search_Object_FindObjectByID";
            copyQuantityPurchaseToIssuedAction.Caption = "Copy quantity";
            copyQuantityPurchaseToIssuedAction.TargetObjectType = typeof(IssuedPurchaseOrderParam);
            copyQuantityPurchaseToIssuedAction.TargetViewType = ViewType.DetailView;
            copyQuantityPurchaseToIssuedAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            copyQuantityPurchaseToIssuedAction.Shortcut = "CtrlL";

            copyQuantityPurchaseToIssuedAction.Execute += CopyQuantityPurchaseToIssuedAction_Execute; ;
        }

        private void CopyQuantityPurchaseToIssuedAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as IssuedPurchaseOrderParam;

            if (viewObject != null &&
                viewObject.Details.Any())
            {
                ListPropertyEditor listPropertyEditor = ((DetailView)View)
                    .FindItem("Details") as ListPropertyEditor;

                var groupCopied = listPropertyEditor.ListView.SelectedObjects
                    .Cast<IssuedPurchaseDetail>();

                if (groupCopied != null &&
                    groupCopied.Any())
                {
                    foreach (var group in groupCopied)
                    {
                        group.IssuedQuantity = group.ReceiptQuantity;
                    }
                }
            }

            View.Refresh();
        }

        private void LoadPurchaseToIssuedAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as IssuedPurchaseOrderParam;

            if (viewObject != null)
            {
                if (viewObject.PurchaseOrder != null)
                {
                    var purchaseGroupLines = viewObject.PurchaseOrder.PurchaseOrderGroupLines;

                    viewObject.Details.Clear();

                    foreach (var groupLine in purchaseGroupLines)
                    {
                        var issuedPurchaseDetail = new IssuedPurchaseDetail()
                        {
                            ItemCode = groupLine.ItemCode,
                            ItemID = groupLine.ItemID,
                            DsmItemID = groupLine.DsmItemID,
                            ItemName = groupLine.ItemName,
                            ItemColorCode = groupLine.ItemColorCode.Replace("\n", " "),
                            ItemColorName = groupLine.ItemColorName.Replace("\n", " "),
                            Position = groupLine.Position,
                            Specify = groupLine.Specify,
                            Season = groupLine.Season,
                            CustomStyle = groupLine.CustomerStyle,
                            LSStyle = string.Join(';', groupLine.PurchaseOrderLines.Select(x => x.LSStyle)),
                            GarmentColorCode = groupLine.GarmentColorCode,
                            GarmentColorName = groupLine.GarmentColorName,
                            GarmentSize = groupLine.GarmentSize,
                            PurchaseQuantity = groupLine.Quantity ?? 0,
                            ReceiptQuantity = groupLine.ReceiptQuantity ?? 0,
                            RemainQuantity = (groupLine.Quantity - groupLine.ReceiptQuantity) ?? 0
                        };

                        viewObject.Details.Add(issuedPurchaseDetail);
                    }

                    View.Refresh();
                }
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
