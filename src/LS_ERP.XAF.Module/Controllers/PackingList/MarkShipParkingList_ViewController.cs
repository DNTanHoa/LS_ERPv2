using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class MarkShipParkingList_ViewController 
        : ObjectViewController<ListView, PackingList>
    {
        public MarkShipParkingList_ViewController()
        {
            InitializeComponent();

            SimpleAction markPackingListAsShip = new SimpleAction(this, "MarkPackingListAsShip", PredefinedCategory.Unspecified);
            markPackingListAsShip.ImageName = "ProductQuickShippments";
            markPackingListAsShip.Caption = "Mark Ship (Ctrl + M)";
            markPackingListAsShip.TargetObjectType = typeof(PackingList);
            markPackingListAsShip.TargetViewType = ViewType.ListView;
            markPackingListAsShip.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            markPackingListAsShip.Shortcut = "CtrlM";

            markPackingListAsShip.Execute += MarkPackingListAsShip_Execute;
        }

        private void MarkPackingListAsShip_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace(typeof(PackingList));
            var inventoryObjectSpace = Application.CreateObjectSpace(typeof(InventoryFG));
            var detailObjectSpace = Application.CreateObjectSpace(typeof(InventoryDetailFG));
            var transObjectSpace = Application.CreateObjectSpace(typeof(FinishGoodTransaction));

            var packingLists = ((ListView)View).SelectedObjects.Cast<PackingList>()
                                .Where(x => x.IsShipped != true && x.CustomerID == "DE").ToList();
            var period = ObjectSpace.GetObjects<InventoryPeriod>()
                            .Where(x => DateTime.Now.Date >= x.FromDate.Date &&
                                        DateTime.Now.Date <= x.ToDate.Date &&
                                        x.StorageCode == "FG").FirstOrDefault();
            
            //if (packingLists.Any())
            //{
            //    try
            //    {
            //        foreach(var data in packingLists)
            //        {
            //            var packingList = objectSpace.GetObjectByKey<PackingList>(data.ID);
            //            packingList.IsShipped = true;
            //            data.IsShipped = true;
                       
            //            foreach (var barcode in packingList.ItemStyles?.FirstOrDefault()?.Barcodes.ToList())
            //            {
            //                var packingLines = packingList?.PackingLines
            //                    .Where(p => p.Size.Trim().Replace(" ","").ToUpper() == barcode.Size.Trim().Replace(" ", "").ToUpper());

            //                var quantity = -1 * packingLines.Sum(p => p.QuantitySize * p.TotalCarton) ?? 0;

            //                if(quantity != 0)
            //                {
            //                    var creatia = CriteriaOperator.Parse("[ItemCode] = ? && [CompanyCode] = ?", barcode.BarCode, packingList.CompanyCode);
            //                    var inventoryFG = inventoryObjectSpace.GetObjects<InventoryFG>(creatia).FirstOrDefault();
            //                    /// Create / Update on hand quantity FG Inventory 
            //                    if (inventoryFG != null)
            //                    {
            //                        inventoryFG.OnHandQuantity += quantity;
            //                        inventoryFG.SetUpdateAudit(SecuritySystem.CurrentUserName);
            //                    }
            //                    else
            //                    {
            //                        inventoryFG = inventoryObjectSpace.CreateObject<InventoryFG>();
            //                        inventoryFG.CustomerStyle = packingList?.ItemStyles?.FirstOrDefault()?.CustomerStyle;
            //                        inventoryFG.GarmentColorCode = packingList?.ItemStyles?.FirstOrDefault()?.ColorCode;
            //                        inventoryFG.GarmentColorName = packingList?.ItemStyles?.FirstOrDefault()?.ColorName;
            //                        inventoryFG.GarmentSize = packingLines?.FirstOrDefault()?.Size;
            //                        inventoryFG.Description = packingList?.ItemStyles?.FirstOrDefault()?.Description;
            //                        inventoryFG.OnHandQuantity = quantity;
            //                        inventoryFG.ItemCode = barcode?.BarCode;
            //                        inventoryFG.ItemName = packingList?.ItemStyles?.FirstOrDefault()?.Description + " " + packingLines?.FirstOrDefault()?.Size;
            //                        inventoryFG.UnitID = "PIECE";
            //                        inventoryFG.CompanyCode = packingList?.CompanyCode;
            //                        inventoryFG.UnitPrice = 0;
            //                        inventoryFG.Season = "";
            //                        inventoryFG.SetCreateAudit(SecuritySystem.CurrentUserName);
            //                        inventoryObjectSpace.CommitChanges();
            //                    }

            //                    /// Create Finish Good Trans
            //                    var finishGoodTrans = transObjectSpace.CreateObject<FinishGoodTransaction>();
            //                    finishGoodTrans.InventoryFGID = inventoryFG.ID;
            //                    finishGoodTrans.PurchaseOrderNumber = packingList?.ItemStyles?.FirstOrDefault()?.PurchaseOrderNumber;
            //                    finishGoodTrans.LSStyle = packingList?.ItemStyles?.FirstOrDefault()?.LSStyle;
            //                    finishGoodTrans.CustomerStyle = packingList?.ItemStyles?.FirstOrDefault()?.CustomerStyle;
            //                    finishGoodTrans.GarmentColorCode = packingList?.ItemStyles?.FirstOrDefault()?.ColorCode;
            //                    finishGoodTrans.GarmentColorName = packingList?.ItemStyles?.FirstOrDefault()?.ColorName;
            //                    finishGoodTrans.GarmentSize = packingLines?.FirstOrDefault()?.Size;
            //                    finishGoodTrans.Season = packingList?.ItemStyles?.FirstOrDefault()?.Season;
            //                    finishGoodTrans.Description = packingList?.ItemStyles?.FirstOrDefault()?.Description;
            //                    finishGoodTrans.TransactionDate = DateTime.Now;
            //                    finishGoodTrans.Quantity = quantity;
            //                    finishGoodTrans.ScanResultDetailID = 0;
            //                    finishGoodTrans.ShippingPlanDetailID = 0;
            //                    finishGoodTrans.PackingListID = packingList.ID;
            //                    finishGoodTrans.InventoryPeriodID = (int)(period?.ID ?? 0);

            //                    /// Create Inventory Detail FG
            //                    var inventotyDetail = detailObjectSpace.CreateObject<InventoryDetailFG>();
            //                    inventotyDetail.InventoryFGID = inventoryFG.ID;
            //                    inventotyDetail.PurchaseOrderNumber = packingList?.ItemStyles?.FirstOrDefault()?.PurchaseOrderNumber;
            //                    inventotyDetail.LSStyle = packingList?.ItemStyles?.FirstOrDefault()?.LSStyle;
            //                    inventotyDetail.CustomerStyle =  packingList?.ItemStyles?.FirstOrDefault()?.CustomerStyle;
            //                    inventotyDetail.GarmentColorCode = packingList?.ItemStyles?.FirstOrDefault()?.ColorCode;
            //                    inventotyDetail.GarmentColorName = packingList?.ItemStyles?.FirstOrDefault()?.ColorName;
            //                    inventotyDetail.GarmentSize = packingLines?.FirstOrDefault()?.Size;
            //                    inventotyDetail.Season = packingList?.ItemStyles?.FirstOrDefault()?.Season;
            //                    inventotyDetail.Description = packingList?.ItemStyles?.FirstOrDefault()?.Description;
            //                    inventotyDetail.TransactionDate = DateTime.Now;
            //                    inventotyDetail.Quantity = quantity;
            //                    inventotyDetail.ScanResultDetailID = 0;
            //                    inventotyDetail.ShippingPlanDetailID = 0;
            //                    inventotyDetail.PackingListID = packingList.ID;
            //                    inventotyDetail.InventoryPeriodID = (int)(period?.ID ?? 0);
            //                    inventotyDetail.SetCreateAudit(SecuritySystem.CurrentUserName);
            //                }
            //            }
            //        }

            //        objectSpace.CommitChanges();
            //        inventoryObjectSpace.CommitChanges();
            //        transObjectSpace.CommitChanges();
            //        detailObjectSpace.CommitChanges();

            //        var message = Message.GetMessageOptions("Mark ship sucessful", "Success", InformationType.Success, null, 5000);
            //        Application.ShowViewStrategy.ShowMessage(message);
            //    }
            //    catch (Exception ex)
            //    {
            //        var message = Message.GetMessageOptions("Mark ship failed", "Error", InformationType.Error, null, 5000);
            //        Application.ShowViewStrategy.ShowMessage(message);
            //    }

            //}

            View.Refresh(true);
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
