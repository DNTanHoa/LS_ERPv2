using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Process;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class CreatePackingList_ViewController : ViewController
    {
        public CreatePackingList_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction popupPacking = new PopupWindowShowAction(this,
                "PopupPacking", PredefinedCategory.Unspecified);
            popupPacking.ImageName = "Header";
            popupPacking.Caption = "Packing Tools";
            popupPacking.TargetObjectType = typeof(PackingList);
            popupPacking.TargetViewType = ViewType.DetailView;
            popupPacking.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupPacking.Shortcut = "CtrlShiftO";

            popupPacking.CustomizePopupWindowParams += PopupPacking_CustomizePopupWindowParams;
            popupPacking.Execute += PopupPacking_Execute;
        }

        private void PopupPacking_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as PackingList;
            var param = e.PopupWindowViewCurrentObject as PackingListCreateParam;
            var newOrdinalShip = 0;
            var sheetNameID = ObjectSpace.GetObjects<PackingSheetName>()
                    .FirstOrDefault(x => x.SheetName.ToUpper().Contains("FAIL")).ID;

            /// Check packing over quantity - PU
            if (viewObject?.CustomerID.Trim().ToUpper() == "PU" &&
                param?.PackingStyles?.ToList()?.Find(x => x?.MultiShip != true) != null)
            {
                var oldPackingLists = new List<PackingList>();
                var tableName = "";
                var joinTableName = "";
                var filter = "";

                if (string.IsNullOrEmpty(tableName))
                    tableName = typeof(PackingList).Name;

                if (string.IsNullOrEmpty(joinTableName))
                    joinTableName = typeof(PackingLine).Name;

                var connectString = Application.ConnectionString ?? string.Empty;
                using (var db = new QueryFactory(
                    new SqlConnection(connectString), new SqlServerCompiler()))
                {
                    filter = " [ID] IN ( SELECT [PACKINGLISTID] FROM " + joinTableName + " WHERE [LSSTYLE] " +
                                " IN (" + string.Join(",", param?.PackingStyles?.Select(x => "'" + x.LSStyle + "'")) + "))" +
                                " AND ISNULL([CONFIRM],0) = 1";

                    oldPackingLists = db.Query(tableName)
                            .WhereRaw(filter).Get<PackingList>().ToList();
                }

                if ((decimal)(oldPackingLists.Sum(x => x.TotalQuantity)
                    + param?.PackingLines?.Sum(x => x.QuantitySize * x.TotalCarton)) 
                    > param?.PackingStyles?.Sum(x => x.TotalQuantity))
                {
                    var error = Message.GetMessageOptions("Exceed order quantity!", "Error",
                                        InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(error);

                    return;
                }
            }

            if (viewObject.PackingLines == null)
                viewObject.PackingLines = new List<PackingLine>();

            if (viewObject.ItemStyles == null)
                viewObject.ItemStyles = new List<ItemStyle>();

            var newPackingLines = viewObject.PackingLines;
            var newCartonNo = 0;
            if (viewObject.PackingLines.Count > 0)
            {
                newCartonNo = viewObject.PackingLines.OrderByDescending(x => x.ToNo).FirstOrDefault().ToNo ?? 0;
            }

            foreach (var packingLine in param.PackingLines)
            {
                if(viewObject.CustomerID != "DE")
                {
                    packingLine.FromNo += newCartonNo;
                    packingLine.ToNo += newCartonNo;
                }
                
                newPackingLines.Add(packingLine);
            }
            newPackingLines.ToList()
                .ForEach(x => x.SequenceNo = newPackingLines.IndexOf(x).ToString("d3"));

            var newPackingStyles = viewObject.ItemStyles;
            foreach (var packingStyle in param.PackingStyles)
            {
                /// Update packing over quantity for multi ship
                if (packingStyle.MultiShip == true)
                {
                    var remainQuantity = param.RemainQuantity;
                    var currentOverQuantities = new List<PackingOverQuantity>();
                    foreach (var orderDetail in packingStyle.OrderDetails)
                    {
                        var overQuantity = new PackingOverQuantity();
                        overQuantity.ColorCode = packingStyle.ColorCode;
                        overQuantity.ColorName = packingStyle.ColorName;
                        overQuantity.Size = orderDetail.Size;
                        overQuantity.ItemStyleNumber = packingStyle.Number;
                        overQuantity.SizeSortIndex = orderDetail.SizeSortIndex;
                        if (param.PackingLines.FirstOrDefault().PrePack.Trim() == "Assorted Size - Solid Color")
                        {
                            overQuantity.Quantity = remainQuantity[packingStyle.Number + ";" + orderDetail.Size] - (decimal)param.PackingLines
                                .Where(x => x.Size == orderDetail.Size && x.LSStyle == packingStyle.LSStyle).Sum(x => x.QuantitySize * x.TotalCarton);
                        }
                        else if (param.PackingLines.FirstOrDefault().PrePack.Trim() == "Solid Size - Assorted Color")
                        {
                            overQuantity.Quantity = remainQuantity[packingStyle.Number + ";" + orderDetail.Size] - (decimal)param.PackingLines
                                .Where(x => x.Size == orderDetail.Size && x.LSStyle == packingStyle.LSStyle).Sum(x => x.QuantitySize * x.TotalCarton);
                        }
                        else
                        {
                            overQuantity.Quantity = remainQuantity[packingStyle.Number + ";" + orderDetail.Size] - (decimal)param.PackingLines
                                .Where(x => x.Size == orderDetail.Size && x.LSStyle == packingStyle.LSStyle).Sum(x => x.TotalQuantity);
                        }

                        currentOverQuantities.Add(overQuantity);
                    }
                    packingStyle.PackingOverQuantities = currentOverQuantities;
                }
                else if (viewObject.CustomerID == "HM")
                {
                    packingStyle.PackingOverQuantities = new List<PackingOverQuantity>();
                }
                newPackingStyles.Add(packingStyle);
            }
            var itemStyle = newPackingStyles.FirstOrDefault();
            if (itemStyle != null)
            {
                if (itemStyle.SalesOrder.CustomerID == "PU")
                {
                    if (!string.IsNullOrEmpty(itemStyle.UCustomerCode)
                    && itemStyle.UCustomerCode.Equals("CAWIN"))
                    {
                        viewObject.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                        {
                            ImageUrl = ConfigurationManager.AppSettings.Get("ShippingImageCanada1").ToString(),
                            SortIndex = 1
                        });

                        viewObject.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                        {
                            ImageUrl = ConfigurationManager.AppSettings.Get("ShippingImageCanada2").ToString(),
                            SortIndex = 2
                        });

                        viewObject.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                        {
                            ImageUrl = ConfigurationManager.AppSettings.Get("DontShortShip").ToString(),
                            SortIndex = 3
                        });
                    }
                    else
                    {
                        viewObject.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                        {
                            ImageUrl = ConfigurationManager.AppSettings.Get("ShippingImageDefault").ToString(),
                            SortIndex = 1
                        });

                        viewObject.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                        {
                            ImageUrl = ConfigurationManager.AppSettings.Get("DontShortShip").ToString(),
                            SortIndex = 2
                        });
                    }

                    if (itemStyle.DeliveryPlace.ToUpper().Equals("Brazil".ToUpper()) || itemStyle.DeliveryPlace.ToUpper().Equals("Israel".ToUpper()) ||
                        itemStyle.DeliveryPlace.ToUpper().Equals("Paraguay".ToUpper()) || itemStyle.DeliveryPlace.ToUpper().Equals("Morocco".ToUpper()))
                    {
                        viewObject.DontShortShip = true;
                    }
                }
                else if (itemStyle.SalesOrder.CustomerID == "HM")
                {
                    viewObject.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                    {
                        ImageUrl = ConfigurationManager.AppSettings.Get("LogoPackingListHM").ToString(),
                        SortIndex = 1
                    });
                }
                else if (itemStyle.SalesOrder.CustomerID == "GA")
                {
                    viewObject.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                    {
                        ImageUrl = ConfigurationManager.AppSettings.Get("LogoPackingListGA").ToString(),
                        SortIndex = 1
                    });
                }
                else if (itemStyle.SalesOrder.CustomerID == "HA")
                {
                    viewObject.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                    {
                        ImageUrl = ConfigurationManager.AppSettings.Get("DontShortShip").ToString(),
                        SortIndex = 1
                    });
                }
                else if (itemStyle.SalesOrder.CustomerID == "DE")
                {
                    viewObject.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                    {
                        ImageUrl = ConfigurationManager.AppSettings.Get("LogoPackingListDE").ToString(),
                        SortIndex = 1
                    });
                }

                if (itemStyle.MultiShip == true)
                {
                    if ((viewObject.OrdinalShip ?? 0) == 0)
                    {
                        var packingLists = objectSpace.GetObjects<PackingList>()
                            .Where(x => (x.LSStyles ?? string.Empty).Contains(itemStyle.LSStyle) &&
                                        (x.SheetNameID ?? 0) != sheetNameID)
                            .OrderByDescending(x => x.OrdinalShip).ToList();
                        newOrdinalShip = 1 + (int)(packingLists.Any() ? packingLists.FirstOrDefault().OrdinalShip ?? 0 : 0);
                    }
                    else
                    {
                        newOrdinalShip = viewObject.OrdinalShip ?? 0;
                    }
                    viewObject.OrdinalShip = newOrdinalShip;
                }
            }

            viewObject.PackingLines = newPackingLines;
            viewObject.ItemStyles = newPackingStyles;
            viewObject.TotalQuantity = newPackingLines.Sum(x => x.QuantitySize * x.TotalCarton);
            viewObject.Confirm = true;
            if (viewObject.ItemStyles.Sum(i => i.TotalQuantity) == viewObject.TotalQuantity)
            {
                viewObject.SheetNameID = 1;
            }
            if (itemStyle.SalesOrder.CustomerID == "GA")
            {
                viewObject.LSStyles = string.Join(";", newPackingStyles.OrderBy(x => x.LSStyle).Select(x => x.LSStyle).Distinct());
            }
            else
            {
                viewObject.LSStyles = string.Join(";", newPackingStyles.Select(x => x.LSStyle).Distinct());
            }

            //// Create box group for scan barcode IFG
            if(viewObject?.CustomerID == "IFG")
            {
                var objectSpaceBoxGroup = Application.CreateObjectSpace(typeof(BoxGroup));
                var errorMsg = "";

                var sheetName = "Total";
                if ((viewObject.SheetNameID ?? 0) != 0)
                    sheetName = ObjectSpace.GetObjectByKey<PackingSheetName>(viewObject.SheetNameID).SheetName;

                var criteria = CriteriaOperator.Parse("[Style] " +
                    "IN (" + string.Join(",", newPackingStyles.Select(x => "'" + x.CustomerStyle + "'")) + ")");
                var itemModels = ObjectSpace.GetObjects<ItemModel>(criteria).ToList();

                criteria = CriteriaOperator.Parse("[ItemStyleNumber] " +
                    "IN (" + string.Join(",", newPackingStyles.Select(x => "'" + x.Number + "'")) + ")");
                var barCodes = ObjectSpace.GetObjects<ItemStyleBarCode>(criteria).ToList();

                var newBoxGroups = IFG_CreateBoxGroupProcess
                        .CreateScanBarcode(ref viewObject, objectSpaceBoxGroup, itemModels, barCodes, sheetName, ref errorMsg);
                if(newBoxGroups != null)
                {
                    /// Set box group -> IsPulled = true when create barcode on packing list
                    var updateBoxGroups = objectSpaceBoxGroup.GetObjects<BoxGroup>()
                        .Where(x => x.PackingListCode == viewObject.PackingListCode &&
                               x.IsPulled != true && x.CustomerID == viewObject.CustomerID).ToList();
                    if (updateBoxGroups.Any())
                    {
                        updateBoxGroups.ForEach(x =>
                        {
                            x.IsPulled = true;
                        });
                    }

                    viewObject.BarCodeCompleted = true;
                    objectSpaceBoxGroup.CommitChanges();
                }
            }
               
            View.Refresh();
        }

        private void PopupPacking_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as PackingList;
            var model = new PackingListCreateParam();
            model.OrdinalShip = viewObject?.OrdinalShip ?? 0;
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = view;
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
