using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class DailyFinishGoodDetail_ViewController 
        : ObjectViewController<ListView, DailyFinishGoodReceiptDetail>
    {
        public DailyFinishGoodDetail_ViewController()
        {
            InitializeComponent();

            SimpleAction copyFinishGoodEntryQuantityAction = new SimpleAction(this, "CopyFinishGoodEntryQuantityAction", PredefinedCategory.Unspecified);
            copyFinishGoodEntryQuantityAction.ImageName = "Copy";
            copyFinishGoodEntryQuantityAction.Caption = "Copy (Shift + C)";
            copyFinishGoodEntryQuantityAction.TargetObjectType = typeof(DailyFinishGoodReceiptDetail);
            copyFinishGoodEntryQuantityAction.TargetViewType = ViewType.ListView;
            copyFinishGoodEntryQuantityAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            copyFinishGoodEntryQuantityAction.Shortcut = "ShiftL";

            copyFinishGoodEntryQuantityAction.Execute += CopyFinishGoodEntryQuantityAction_Execute;

            SimpleAction copyFinishGoodScanResultAction = new SimpleAction(this, "CopyFinishGoodScanResultAction", PredefinedCategory.Unspecified);
            copyFinishGoodScanResultAction.ImageName = "Copy";
            copyFinishGoodScanResultAction.Caption = "Scan Result (Shift + S)";
            copyFinishGoodScanResultAction.TargetObjectType = typeof(DailyFinishGoodReceiptDetail);
            copyFinishGoodScanResultAction.TargetViewType = ViewType.ListView;
            copyFinishGoodScanResultAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            copyFinishGoodScanResultAction.Shortcut = "ShiftS";

            copyFinishGoodScanResultAction.Execute += CopyFinishGoodScanResultAction_Execute;
        }

        private void CopyFinishGoodScanResultAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CopyFinishGoodEntryQuantityAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObjects = View.SelectedObjects.Cast<DailyFinishGoodReceiptDetail>();
            var masterObject = (((DevExpress.ExpressApp.ListView)View).CollectionSource as PropertyCollectionSource)
                             .MasterObject as DailyFinishGoodReceiptParam;

            if (viewObjects != null &&
               viewObjects.Any())
            {
                var itemStyleBarCodes = ObjectSpace.GetObjects<ItemStyle>(
                     CriteriaOperator.Parse("Contains(PurchaseOrderNumber, ?) AND [SalesOrder.CustomerID] = ?", 
                     masterObject.PurchaseOrderNumber, masterObject.Customer?.ID))
                    .SelectMany(x => x.Barcodes);

                foreach(var detail in viewObjects)
                {
                    var barCode = itemStyleBarCodes.FirstOrDefault(x => x.Size == detail.GarmentSize &&
                                                                        detail.PurchaseOrderNumber == x.ItemStyle?.PurchaseOrderNumber);
                    
                    if(barCode != null)
                    {
                        detail.EntryQuantity = detail.OrderQuantity;
                        detail.CartonQuantity = Math.Ceiling(detail.OrderQuantity / decimal.Parse(barCode.PCB));

                        View.Refresh();
                    }
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
