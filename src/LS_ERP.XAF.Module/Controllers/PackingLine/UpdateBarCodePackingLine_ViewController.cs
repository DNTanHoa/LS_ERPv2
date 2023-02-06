using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class UpdateBarCodePackingLine_ViewController : ObjectViewController<ListView, PackingLine>
    {
        public UpdateBarCodePackingLine_ViewController()
        {
            InitializeComponent();

            //SimpleAction updateBarCodePackingLineAction = new SimpleAction(this, "UpdateBarCodePackingLineAction", PredefinedCategory.Unspecified);
            //updateBarCodePackingLineAction.ImageName = "Barcode_32x32";
            //updateBarCodePackingLineAction.Caption = "Update BarCode (Ctrl + B)";
            //updateBarCodePackingLineAction.TargetObjectType = typeof(PackingLine);
            //updateBarCodePackingLineAction.TargetViewType = ViewType.ListView;
            //updateBarCodePackingLineAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //updateBarCodePackingLineAction.Shortcut = "CtrlB";

            PopupWindowShowAction updateBarCodePackingLineAction = new PopupWindowShowAction(this, "UpdateBarCodePackingLineAction", PredefinedCategory.Unspecified);
            updateBarCodePackingLineAction.ImageName = "Barcode_32x32";
            updateBarCodePackingLineAction.Caption = "Update BarCode (Ctrl + B)";
            updateBarCodePackingLineAction.TargetObjectType = typeof(PackingLine);
            updateBarCodePackingLineAction.TargetViewType = ViewType.ListView;
            updateBarCodePackingLineAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            updateBarCodePackingLineAction.Shortcut = "CtrlB";

            updateBarCodePackingLineAction.CustomizePopupWindowParams += UpdateBarCodePackingLineAction_CustomizePopupWindowParams;
            updateBarCodePackingLineAction.Execute += UpdateBarCodePackingLineAction_Execute;
        }

        private void UpdateBarCodePackingLineAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var objectSpaceItemModel = Application.CreateObjectSpace(typeof(ItemModel));
            var objectSpaceBoxGroup = Application.CreateObjectSpace(typeof(BoxGroup));
            var packingLines = (View as ListView).CollectionSource.List
                    .Cast<PackingLine>().ToList().OrderBy(x => x.SequenceNo);
            var param = e.PopupWindowViewCurrentObject as ViewPackingLineBarCodeParam;
            var packingList = ((DetailView)View.ObjectSpace.Owner).CurrentObject as PackingList;
            var errorMessage = "";

            if (packingLines.FirstOrDefault().PrePack.Trim() == "Assorted Size - Solid Color")
            {
                var barCode = param?.BarCodes?.FirstOrDefault()?.BarCode;
                if(!string.IsNullOrEmpty(barCode))
                {
                    packingLines.ToList().ForEach(x =>
                    {
                        x.BarCode = barCode.Trim().Replace(" ", "");
                    });
                }
            }
            else
            {
                param.BarCodes.ToList().ForEach(b =>
                {
                    packingLines.Where(x => x.Size == b.Size).ToList().ForEach(x =>
                    {
                        if(!string.IsNullOrEmpty(b.BarCode))
                        {
                            x.BarCode = b.BarCode.Trim().Replace(" ", "");
                        }
                    });
                });
            }

            /// Process pack validation GA
            if (packingList.CustomerID == "GA")
            {
                bool checkAssortedSize = false;
                string message = "";
                var totalBarCodeCarton = 0;
                if (packingLines.OrderBy(x => x.SequenceNo).FirstOrDefault().PrePack.Trim() == "Assorted Size - Solid Color")
                    checkAssortedSize = true;
                else
                    checkAssortedSize = false;
                var styles = packingList.ItemStyles.Distinct().ToList();
                var boxGroup = objectSpaceBoxGroup.CreateObject<BoxGroup>();
                var PONumber = styles.FirstOrDefault()?.PurchaseOrderNumber ?? string.Empty;
                var firstRow = true;
                var barcodes = "";
                var criteria = CriteriaOperator.Parse("[Style] " +
                    "IN (" + string.Join(",", styles.Select(x => "'" + x.CustomerStyle + "'")) + ")");
                List<ItemModel> itemModels = objectSpaceItemModel.GetObjects<ItemModel>(criteria).ToList();
                var sortPackingLines = packingLines.Where(x => x.LSStyle == styles.FirstOrDefault().LSStyle)
                                        .OrderBy(x => x.SequenceNo).ToList();

                /// Set box group -> IsPulled = true when update barcode on packing list
                var boxGroups = objectSpaceBoxGroup.GetObjects<BoxGroup>()
                    .Where(x => x.PackingListCode == packingList.PackingListCode && 
                           x.IsPulled != true && x.CustomerID == packingList.CustomerID).ToList();
                if (boxGroups.Any())
                {
                    boxGroups.ForEach(x =>
                    {
                        x.IsPulled = true;
                    });
                }

                var sheetName = "Total";
                if ((packingList.SheetNameID ?? 0) != 0)
                    sheetName = ObjectSpace.GetObjectByKey<PackingSheetName>(packingList.SheetNameID).SheetName;

                foreach (var packingLine in sortPackingLines.Where(x => !string.IsNullOrEmpty(x.BarCode)))
                {
                    /// Create Box Group
                    if (firstRow)
                    {
                        boxGroup.PONum = PONumber;
                        boxGroup.Date = DateTime.Now;
                        boxGroup.FileName = String.Format("Pulled from ERP ({0})", packingList.LSStyles);
                        boxGroup.PackingListCode = packingList.PackingListCode;
                        boxGroup.CustomerID = packingList.CustomerID;
                        boxGroup.SheetName = sheetName;
                        boxGroup.SetCreateAudit(SecuritySystem.CurrentUserName);
                        firstRow = false;

                        /// Insert barcode
                        barcodes = packingLine?.BarCode;
                        if (!string.IsNullOrEmpty(barcodes))
                        {
                            if (!InsertBarcodes(barcodes.Trim(), ref boxGroup, ref errorMessage))
                            {
                                message = errorMessage;
                                break;
                            }
                            totalBarCodeCarton += boxGroup.BoxModels.Count();
                            packingLine.TotalBarCode = boxGroup.BoxModels.Count();
                        }
                    }
                    else if (!checkAssortedSize)
                    {
                        boxGroup = objectSpaceBoxGroup.CreateObject<BoxGroup>();
                        boxGroup.PONum = PONumber;
                        boxGroup.Date = DateTime.Now;
                        boxGroup.FileName = String.Format("Pulled from ERP ({0})", packingList.LSStyles);
                        boxGroup.PackingListCode = packingList.PackingListCode;
                        boxGroup.CustomerID = packingList.CustomerID;
                        boxGroup.SetCreateAudit(SecuritySystem.CurrentUserName);
                        boxGroup.SheetName = sheetName;

                        /// Insert barcode
                        barcodes = packingLine?.BarCode;
                        if (!string.IsNullOrEmpty(barcodes))
                        {
                            if (!InsertBarcodes(barcodes.Trim(), ref boxGroup, ref errorMessage))
                            {
                                message = errorMessage;
                                break;
                            }
                            totalBarCodeCarton += boxGroup.BoxModels.Count();
                            packingLine.TotalBarCode = boxGroup.BoxModels.Count();
                        }
                    }

                    /// Insert box detail
                    if (!InsertBoxDetail(packingLine, itemModels, styles, checkAssortedSize, ref boxGroup, ref errorMessage))
                    {
                        message = errorMessage;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(message))
                {
                    packingLines.ToList().ForEach(x =>
                    {
                        x.BarCode = null;
                    });
                    var error = Message.GetMessageOptions(message, "Error",
                            InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(error);
                }
                else
                {
                    packingList.TotalBarCodeCarton = totalBarCodeCarton;
                    objectSpace.CommitChanges();
                    objectSpaceBoxGroup.CommitChanges();

                    View.Refresh();
                    ((DetailView)View.ObjectSpace.Owner).Refresh();
                }
            }
        }
        private void UpdateBarCodePackingLineAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var packingLines = ((ListView)View).CollectionSource.List
                    .Cast<PackingLine>().OrderBy(x => x.SequenceNo).ToList();
           
            var model = new ViewPackingLineBarCodeParam();
            if(packingLines.Any())
            {
                var barCodes = new List<BarCodeForPacking>();
                var packingLine = packingLines.FirstOrDefault();
                if (packingLine.PrePack.Trim() == "Assorted Size - Solid Color")
                {
                    var barCode = new BarCodeForPacking();
                    barCode.FromNo = packingLine?.FromNo ?? 0;
                    barCode.ToNo = packingLine?.ToNo ?? 0;
                    barCode.TotalCarton = packingLine?.TotalCarton ?? 0;
                    barCode.BarCode = packingLine.BarCode;
                    barCode.PrePack = packingLine.PrePack;

                    barCodes.Add(barCode);
                }
                else
                {
                    var sortedPackingLines = packingLines.Where(x => x.LSStyle == packingLine.LSStyle)
                                                .OrderBy(x => x.SequenceNo).ToList();
                    foreach (var data in sortedPackingLines)
                    {
                        var barCode = new BarCodeForPacking();
                        barCode.FromNo = data?.FromNo ?? 0;
                        barCode.ToNo = data?.ToNo ?? 0;
                        barCode.TotalCarton = data?.TotalCarton ?? 0;
                        barCode.Size = data.Size;
                        barCode.BarCode = data.BarCode;
                        barCode.PrePack = data.PrePack;

                        barCodes.Add(barCode);
                    }
                }
                model.BarCodes = barCodes;
            }
            
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
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
        private bool InsertBarcodes(string barcodes, ref BoxGroup boxGroup, ref string errorMessage)
        {
            if (!String.IsNullOrEmpty(barcodes))
            {
                barcodes = barcodes.Trim();

                //barcodes = barcodes.Replace(">", "").Replace("》", "").Replace(",", "").Replace(";", "\n");
                string fakebarcodes = barcodes.Replace(">", "").Replace("》", "").Replace(",", "\n")
                                              .Replace(";", "\n").Replace("\n\n", "\n").Replace("  ", " ")
                                              .Replace("   ", " ").Replace(" ", "\n").Replace("\"", "");

                string[] arrBarcodes = fakebarcodes.Split(
                                                        new[] { "\r\n", "\r", "\n", " " },
                                                        StringSplitOptions.None
                                                    );
                foreach (var item in arrBarcodes)
                {
                    boxGroup.BarcodeRange += item + "\n";
                }

                for (int ii = 0; ii < arrBarcodes.Count(); ii++)
                {
                    string barcode = arrBarcodes[ii].Trim();
                    if (!String.IsNullOrEmpty(barcode))
                    {
                        long startBC = 0;
                        long endBC = 0;
                        if (barcode.IndexOf("-") > 0)
                        {
                            string[] rangeBarcode = barcode.Split('-');
                            startBC = long.Parse(rangeBarcode[0]);
                            endBC = long.Parse(rangeBarcode[1]);
                        }
                        else
                        {
                            startBC = long.Parse(barcode);
                            endBC = long.Parse(barcode);
                        }

                        if (startBC > endBC)
                        {
                            long temp = 0;
                            temp = startBC;
                            startBC = endBC;
                            endBC = temp;

                            if (startBC.ToString().Length < endBC.ToString().Length)
                            {
                                errorMessage = String.Format("Barcode {0} incorrect, please re-check!!!", barcodes);
                                return false;
                            }
                        }

                        // generate barcode
                        bool isLoop = true;
                        do
                        {
                            var boxModel = new BoxModel();
                            boxModel.Barcode = startBC.ToString();
                            boxModel.SetCreateAudit(SecuritySystem.CurrentUserName);
                            boxGroup.BoxModels.Add(boxModel);

                            if (startBC == endBC)
                            {
                                isLoop = false;
                            }

                            startBC++;

                        } while (isLoop);
                    }
                }

            }
            return true;
        }

        private bool InsertBoxDetail(PackingLine packingLine, List<ItemModel> itemModels,
            List<ItemStyle> styles, bool checkAssortedSize, ref BoxGroup boxGroup, ref string errorMessage)
        {
            foreach (var style in styles)
            {
                var itemModel = itemModels.Where(x => x.Style.Trim().ToUpper().Equals(style.CustomerStyle.Trim().ToUpper()) &&
                                                 x.GarmentColorCode.Trim().ToUpper().Equals(style.ColorCode.Trim().ToUpper()) &&
                                                 x.GarmentSize.Trim().ToUpper().Equals(packingLine.Size.Trim().ToUpper())).FirstOrDefault();
                if (itemModel != null)
                {
                    if(!boxGroup.BoxDetails.Select(d => d.GTIN).Contains(itemModel.UPC))
                    {
                        var boxDetail = new BoxDetail();
                        boxDetail.Style = itemModel.Style;
                        boxDetail.Color = itemModel.GarmentColorCode;
                        boxDetail.ColorName = itemModel.GarmentColorName;
                        boxDetail.Description = style.Description;
                        boxDetail.Size = itemModel.GarmentSize;
                        boxDetail.GTIN = itemModel.UPC;
                        boxDetail.IsMerge = true;
                        boxDetail.Qty = checkAssortedSize ? (int)packingLine.Quantity : (int)packingLine.QuantityPerCarton;
                        boxDetail.SetCreateAudit(SecuritySystem.CurrentUserName);
                        boxGroup.BoxDetails.Add(boxDetail);
                    }
                }
                else
                {
                    errorMessage = String.Format("Style: {0} - Color: {1} - Size: {2} not found UPC, please re-check!!!",
                                                style.CustomerStyle, style.ColorCode, packingLine.Size);
                    return false;
                }
            }
            return true;
        }
    }
}
