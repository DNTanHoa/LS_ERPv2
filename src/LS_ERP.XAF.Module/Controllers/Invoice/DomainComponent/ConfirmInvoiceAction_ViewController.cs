using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class ConfirmInvoiceAction_ViewController : ObjectViewController<ListView, Invoice>
    {
        private List<PackingList> ShippedList;
        private List<PackingList> originList;
        public ConfirmInvoiceAction_ViewController()
        {
            InitializeComponent();

            SimpleAction confirmInvoice = new SimpleAction(this, "ConfirmInvoice", PredefinedCategory.Unspecified);
            confirmInvoice.ImageName = "ApplyChanges";
            confirmInvoice.Caption = "Confirm";
            confirmInvoice.TargetObjectType = typeof(Invoice);
            confirmInvoice.TargetViewType = ViewType.ListView;
            confirmInvoice.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            confirmInvoice.Execute += ConfirmInvoice_Execute;
        }
        private void ConfirmInvoice_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace(typeof(Invoice));
            var packingObjectSpace = Application.CreateObjectSpace(typeof(PackingList));
            var inventoryObjectSpace = Application.CreateObjectSpace(typeof(InventoryFG));
            //var detailObjectSpace = Application.CreateObjectSpace(typeof(InventoryDetailFG));
            //var transObjectSpace = Application.CreateObjectSpace(typeof(FinishGoodTransaction));
            var errorMessage = "";
            ShippedList = new List<PackingList>();
            originList = new List<PackingList>();

            var invoices = ((ListView)View).SelectedObjects.Cast<Invoice>()
                        .Where(x => x.IsConfirmed != true).ToList();

            if (invoices.Any())
            {
                try
                {
                    ShippedList.AddRange(invoices.SelectMany(x => x?.PackingList));

                    invoices.ForEach(x =>
                    {
                        var invoice = objectSpace.GetObjectByKey<Invoice>(x.ID);

                        /// Create / Update on hand quantity FG Inventory 
                        if (x.CustomerID == "DE")
                        {
                            var period = ObjectSpace.GetObjects<InventoryPeriod>()
                                            .Where(x => DateTime.Now.Date >= x.FromDate.Date &&
                                                        DateTime.Now.Date <= x.ToDate.Date &&
                                                        x.StorageCode == "FG")?.FirstOrDefault();

                            if (x.PackingList.Any())
                            {
                                try
                                {
                                    foreach (var data in x.PackingList)
                                    {
                                        var packingList = packingObjectSpace.GetObjectByKey<PackingList>(data.ID);
                                        packingList.IsShipped = true;

                                        foreach (var barcode in packingList.ItemStyles?.FirstOrDefault()?.Barcodes.ToList())
                                        {
                                            var packingLines = packingList?.PackingLines
                                                .Where(p => p.Size.Trim().Replace(" ", "").ToUpper() == barcode.Size.Trim().Replace(" ", "").ToUpper());

                                            var quantity = -1 * packingLines.Sum(p => p.QuantitySize * p.TotalCarton) ?? 0;

                                            if (quantity != 0)
                                            {
                                                var creatia = CriteriaOperator.Parse("[ItemCode] = ? && [CompanyCode] = ?", barcode.BarCode, packingList.CompanyCode);
                                                var inventoryFG = inventoryObjectSpace.GetObjects<InventoryFG>(creatia).FirstOrDefault();
                                                /// Create / Update on hand quantity FG Inventory 
                                                if (inventoryFG != null)
                                                {
                                                    inventoryFG.OnHandQuantity += quantity;
                                                    inventoryFG.SetUpdateAudit(SecuritySystem.CurrentUserName);
                                                }
                                                else
                                                {
                                                    inventoryFG = inventoryObjectSpace.CreateObject<InventoryFG>();
                                                    inventoryFG.CustomerStyle = packingList?.ItemStyles?.FirstOrDefault()?.CustomerStyle;
                                                    inventoryFG.GarmentColorCode = packingList?.ItemStyles?.FirstOrDefault()?.ColorCode;
                                                    inventoryFG.GarmentColorName = packingList?.ItemStyles?.FirstOrDefault()?.ColorName;
                                                    inventoryFG.GarmentSize = packingLines?.FirstOrDefault()?.Size;
                                                    inventoryFG.Description = packingList?.ItemStyles?.FirstOrDefault()?.Description;
                                                    inventoryFG.OnHandQuantity = quantity;
                                                    inventoryFG.ItemCode = barcode?.BarCode;
                                                    inventoryFG.ItemName = packingList?.ItemStyles?.FirstOrDefault()?.Description + " " + packingLines?.FirstOrDefault()?.Size;
                                                    inventoryFG.UnitID = "PIECE";
                                                    inventoryFG.CompanyCode = packingList?.CompanyCode;
                                                    inventoryFG.UnitPrice = 0;
                                                    inventoryFG.Season = "";
                                                    inventoryFG.SetCreateAudit(SecuritySystem.CurrentUserName);
                                                    inventoryFG.FinishGoodTransactions = new List<FinishGoodTransaction>();
                                                    inventoryFG.InventoryDetailFG = new List<InventoryDetailFG>();
                                                    //inventoryObjectSpace.CommitChanges();
                                                }

                                                /// Create Finish Good Trans
                                                var finishGoodTrans = new FinishGoodTransaction(); //transObjectSpace.CreateObject<FinishGoodTransaction>();
                                                finishGoodTrans.InventoryFGID = inventoryFG.ID;
                                                finishGoodTrans.PurchaseOrderNumber = packingList?.ItemStyles?.FirstOrDefault()?.PurchaseOrderNumber;
                                                finishGoodTrans.LSStyle = packingList?.ItemStyles?.FirstOrDefault()?.LSStyle;
                                                finishGoodTrans.CustomerStyle = packingList?.ItemStyles?.FirstOrDefault()?.CustomerStyle;
                                                finishGoodTrans.GarmentColorCode = packingList?.ItemStyles?.FirstOrDefault()?.ColorCode;
                                                finishGoodTrans.GarmentColorName = packingList?.ItemStyles?.FirstOrDefault()?.ColorName;
                                                finishGoodTrans.GarmentSize = packingLines?.FirstOrDefault()?.Size;
                                                finishGoodTrans.Season = packingList?.ItemStyles?.FirstOrDefault()?.Season;
                                                finishGoodTrans.Description = packingList?.ItemStyles?.FirstOrDefault()?.Description;
                                                finishGoodTrans.TransactionDate = DateTime.Now;
                                                finishGoodTrans.Quantity = quantity;
                                                //finishGoodTrans.ScanResultDetailID = 0;
                                                //finishGoodTrans.ShippingPlanDetailID = 0;
                                                finishGoodTrans.PackingListID = packingList.ID;
                                                finishGoodTrans.InventoryPeriodID = (int)(period?.ID ?? 0);
                                                inventoryFG.FinishGoodTransactions.Add(finishGoodTrans);    

                                                /// Create Inventory Detail FG
                                                var inventoryDetail = new InventoryDetailFG(); //detailObjectSpace.CreateObject<InventoryDetailFG>();
                                                inventoryDetail.InventoryFGID = inventoryFG.ID;
                                                inventoryDetail.PurchaseOrderNumber = packingList?.ItemStyles?.FirstOrDefault()?.PurchaseOrderNumber;
                                                inventoryDetail.LSStyle = packingList?.ItemStyles?.FirstOrDefault()?.LSStyle;
                                                inventoryDetail.CustomerStyle = packingList?.ItemStyles?.FirstOrDefault()?.CustomerStyle;
                                                inventoryDetail.GarmentColorCode = packingList?.ItemStyles?.FirstOrDefault()?.ColorCode;
                                                inventoryDetail.GarmentColorName = packingList?.ItemStyles?.FirstOrDefault()?.ColorName;
                                                inventoryDetail.GarmentSize = packingLines?.FirstOrDefault()?.Size;
                                                inventoryDetail.Season = packingList?.ItemStyles?.FirstOrDefault()?.Season;
                                                inventoryDetail.Description = packingList?.ItemStyles?.FirstOrDefault()?.Description;
                                                inventoryDetail.TransactionDate = DateTime.Now;
                                                inventoryDetail.Quantity = quantity;
                                                //inventoryDetail.ScanResultDetailID = 0;
                                                //inventoryDetail.ShippingPlanDetailID = 0;
                                                inventoryDetail.PackingListID = packingList.ID;
                                                inventoryDetail.InventoryPeriodID = (int)(period?.ID ?? 0);
                                                inventoryDetail.SetCreateAudit(SecuritySystem.CurrentUserName);
                                                inventoryFG.InventoryDetailFG.Add(inventoryDetail);
                                            }
                                        }
                                    }
                                //    transObjectSpace.CommitChanges();
                                //    detailObjectSpace.CommitChanges();
                                }
                                catch (Exception ex)
                                {
                                    errorMessage = ex.Message;
                                    //var message = Message.GetMessageOptions("Confirm failed", "Error", InformationType.Error, null, 5000);
                                    //Application.ShowViewStrategy.ShowMessage(message);
                                }
                            }
                        }
                        else
                        {
                            foreach(var data in invoice.PackingList)
                            {
                                var packingList = packingObjectSpace.GetObjectByKey<PackingList>(data.ID);
                                packingList.IsShipped = true;

                                if((packingList?.ParentPackingListID ?? 0) > 0)
                                {
                                    var origin = CheckFullShipped(packingList);
                                    if(origin != null)
                                    {
                                        var updateOrigin = packingObjectSpace.GetObjectByKey<PackingList>(origin.ID);
                                        updateOrigin.IsShipped = true;

                                        originList.Add(origin);
                                    }
                                }
                            }
                            //packingObjectSpace.CommitChanges();
                        }

                        invoice.IsConfirmed = true;
                        invoice.SetUpdateAudit(SecuritySystem.CurrentUserName);
                        x.IsConfirmed = true;
                    });
                }
                catch (Exception ex)
                {
                    errorMessage =ex.Message;
                }
            }

            if(!string.IsNullOrEmpty(errorMessage))
            {
                var message = Message.GetMessageOptions(errorMessage, "Error", InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(message);
            }
            else
            {
                inventoryObjectSpace.CommitChanges();
                packingObjectSpace.CommitChanges();
                objectSpace.CommitChanges();

                var message = Message.GetMessageOptions("Confirm sucessful", "Success", InformationType.Success, null, 5000);
                Application.ShowViewStrategy.ShowMessage(message);
            }

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

        public PackingList CheckFullShipped(PackingList packingList)
        {
            var resultList = new List<PackingList>();
            var parentID = packingList?.ParentPackingListID ?? 0;
            var parentPackingList = new PackingList();
            decimal currentQuantity = 0;

            var tableName = typeof(PackingList).Name;
            var connectString = Application.ConnectionString ?? string.Empty;
            var db = new QueryFactory(new SqlConnection(connectString), new SqlServerCompiler());

            while (parentID > 0)
            {
                parentPackingList =  db.Query(tableName)
                        .WhereRaw(" [ID] =  " + parentID).Get<PackingList>().FirstOrDefault();
                parentID = parentPackingList?.ParentPackingListID ?? 0;
            }
            
            if(!originList.Select(x => x.ID).Contains(parentPackingList?.ID ?? 0))
            {
                parentID = parentPackingList.ID;

                while (parentID > 0)
                {
                    var checkList = db.Query(tableName)
                            .WhereRaw(" [ParentPackingListID] =  " + parentID).Get<PackingList>().ToList();

                    resultList.AddRange(checkList.Where(x => x.IsSeparated != true).ToList());

                    parentID = checkList?.FirstOrDefault(x => x.IsSeparated == true)?.ID ?? 0;
                }

                ShippedList.ForEach(x =>
                {
                    if (resultList.Select(r => r.ID).Contains(x.ID))
                    {
                        currentQuantity += x.TotalQuantity ?? 0;
                    }
                });


                if (parentPackingList.TotalQuantity ==
                    (resultList?.Where(x => x.IsShipped == true)?.Sum(x => x.TotalQuantity) + currentQuantity))
                {
                    return parentPackingList;
                }
            }
            return null;
        }
    }

}
