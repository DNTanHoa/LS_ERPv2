using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Process;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class ClonePackingList_ViewController : ViewController
    {
        public ClonePackingList_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction popupClonePackingList = new PopupWindowShowAction(this, "ClonePackingList", PredefinedCategory.Unspecified);
            popupClonePackingList.ImageName = "Copy";
            popupClonePackingList.Caption = "Clone (Ctrl + C)";
            popupClonePackingList.TargetObjectType = typeof(PackingList);
            popupClonePackingList.TargetViewType = ViewType.ListView;
            popupClonePackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupClonePackingList.Shortcut = "CtrlC";

            popupClonePackingList.CustomizePopupWindowParams +=
                PopupClonePackingList_CustomizePopupWindowParams;
            popupClonePackingList.Execute += PopupClonePackingList_Execute;
        }
        private void PopupClonePackingList_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace();
            var packingListID = ((PackingList)View.CurrentObject).ID;
            var viewObject = objectSpace.GetObjectByKey<PackingList>(packingListID);

            var styleList = "";
            if(viewObject.CustomerID == "IFG")
            {
                styleList = viewObject?.ItemStyles?.First()?.ContractNo?.Trim();
            }
            else
            {
                viewObject.ItemStyles.ToList().ForEach(x =>
                {
                    if (string.IsNullOrEmpty(styleList.Trim()))
                    {
                        styleList = x.CustomerStyle.Trim();
                    }
                    else if (!styleList.Contains(x.CustomerStyle))
                    {
                        styleList += ";" + x.CustomerStyle;
                    }
                });
            }

            var model = objectSpace.CreateObject<PackingList>();
            model.PackingListCode = "PK-" + Nanoid.Nanoid.Generate("ABCDEFGHIJKLMNOPQRSTUV123456789", 6);
            model.PackingListDate = DateTime.Now;
            model.CompanyCode = "LS";
            model.RatioLeft = viewObject.RatioLeft;
            model.RatioRight = viewObject.RatioRight;
            model.RatioBottom = viewObject.RatioBottom;
            model.Confirm = viewObject.Confirm;
            model.DeliveryGroup = viewObject.DeliveryGroup;
            model.TotalQuantity = viewObject.TotalQuantity;
            model.DontShortShip = viewObject.DontShortShip;
            model.LSStyles = styleList;
            model.Factory = viewObject.Factory;
            model.ShippingMark1 = viewObject.ShippingMark1;
            model.ShippingMark1Code = viewObject.ShippingMark1Code;
            model.ShippingMark2 = viewObject.ShippingMark2;
            model.ShippingMark2Code = viewObject.ShippingMark2Code;
            model.ShippingMethod = viewObject.ShippingMethod;
            model.ShippingMethodCode = viewObject.ShippingMethodCode;
            model.SetCreateAudit(SecuritySystem.CurrentUserName);
            model.Company = viewObject.Company;
            model.Customer = viewObject.Customer;
            model.CustomerID = viewObject.CustomerID;
            model.SalesOrderID = viewObject.SalesOrderID;
            model.PackingLines = viewObject.PackingLines;
            //model.PackingListImageThumbnails = viewObject.PackingListImageThumbnails;
            //model.ItemStyles = itemStyles;
            model.PackingUnit = viewObject.PackingUnit;

            var view = Application.CreateDetailView(objectSpace, model, true);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = view;
        }
        private void PopupClonePackingList_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

            var packingLists = ((ListView)View).CollectionSource.List;
            var param = e.PopupWindowViewCurrentObject as PackingList;
            var errorMessage = "";

            if (param.PackingLines.Any() && param.TotalQuantity > 0)
            {
                var prePack = param.PackingLines.ToList().First().PrePack.Trim();
                if (prePack == "Assorted Size - Solid Color")
                {
                    if(param.TotalQuantity != param.PackingLines.Sum(x => x.QuantitySize * x.TotalCarton))
                    {
                        errorMessage = "Please update packing line before";
                    }
                }
                else
                {
                    foreach (var packingLine in param.PackingLines.Where(x => x.QuantitySize > 0))
                    {
                        if ((int)(packingLine.TotalQuantity / packingLine.QuantitySize) != packingLine.TotalCarton)
                        {
                            errorMessage = "Please update packing line before";
                            break;
                        }
                    }
                }
            }
            else
            {
                errorMessage = "Out of remaining quantity";
            }

            if(!string.IsNullOrEmpty(errorMessage.Trim()))
            {
                var error = Message.GetMessageOptions(errorMessage, "Error",
                    InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(error);
                return;
            }
            else
            {
                ///// Update packing over quantity for multi ship
                if (param.ItemStyles.First().MultiShip == true)
                {
                    if(param.CustomerID == "IFG")
                    {
                        //param.ItemStyles.First().PackingOverQuantities.ToList().ForEach(x =>
                        //{
                        //    if (param.PackingLines.FirstOrDefault().PrePack.Trim() == "Assorted Size - Solid Color")
                        //    {
                        //        x.Quantity -= (int)param.PackingLines.Where(y => y.Size == x.Size).Sum(y => y.QuantitySize * y.TotalCarton);
                        //    }
                        //    else
                        //    {
                        //        x.Quantity -= (int)param.PackingLines.Where(y => y.Size == x.Size).Sum(y => y.TotalQuantity);
                        //    }
                        //});
                        foreach (var itemStyle in param.ItemStyles)
                        {
                            itemStyle.PackingOverQuantities.ToList().ForEach(x =>
                            {
                                if (param.PackingLines.FirstOrDefault().PrePack.Trim() == "Assorted Size - Solid Color")
                                {
                                    x.Quantity -= (int)param.PackingLines.Where(y => y.LSStyle == itemStyle.LSStyle && y.Size == x.Size).Sum(y => y.QuantitySize * y.TotalCarton);
                                }
                                else
                                {
                                    x.Quantity -= (int)param.PackingLines.Where(y => y.LSStyle == itemStyle.LSStyle && y.Size == x.Size).Sum(y => y.TotalQuantity);
                                }
                            });
                        }
                    }
                    else if (param.CustomerID == "GA")
                    {
                        param.ItemStyles.ToList().ForEach(i =>
                        {
                            i.PackingOverQuantities.ToList().ForEach(x =>
                            {
                                if (param.PackingLines.FirstOrDefault().PrePack.Trim() == "Assorted Size - Solid Color")
                                {
                                    x.Quantity -= (int)param.PackingLines
                                        .Where(y => y.Size == x.Size && y.LSStyle == i.LSStyle).Sum(y => y.QuantitySize * y.TotalCarton);
                                }
                                else
                                {
                                    x.Quantity -= (int)param.PackingLines
                                        .Where(y => y.Size == x.Size && y.LSStyle == i.LSStyle).Sum(y => y.TotalQuantity);
                                }
                            });
                        });
                    }
                    else if (param.CustomerID == "DE")
                    {
                        param.ItemStyles.First().PackingOverQuantities.ToList().ForEach(x =>
                        {
                            if (param.PackingLines.FirstOrDefault().PrePack.Trim() == "Assorted Size - Solid Color")
                            {
                                x.Quantity -= (int)param.PackingLines.Where(y => y.Size == x.Size).Sum(y => y.QuantitySize * y.TotalCarton);
                            }
                            else
                            {
                                x.Quantity -= (int)param.PackingLines.Where(y => y.Size == x.Size).Sum(y => y.TotalQuantity);
                            }
                        });
                    }
                }

                if (param.CustomerID == "GA")
                {
                    param.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                    {
                        ImageUrl = ConfigurationManager.AppSettings.Get("LogoPackingListGA").ToString(),
                        SortIndex = 1
                    });
                }
                else if (param.CustomerID == "DE")
                {
                    param.PackingListImageThumbnails.Add(new PackingListImageThumbnail
                    {
                        ImageUrl = ConfigurationManager.AppSettings.Get("LogoPackingListDE").ToString(),
                        SortIndex = 1
                    });
                }

                //// Create box group for scan barcode IFG
                if (param?.CustomerID == "IFG")
                {
                    var objectSpaceBoxGroup = Application.CreateObjectSpace(typeof(BoxGroup));
                    var errorMsg = "";

                    var sheetName = "Total";
                    if ((param.SheetNameID ?? 0) != 0)
                        sheetName = ObjectSpace.GetObjectByKey<PackingSheetName>(param.SheetNameID).SheetName;

                    var criteria = CriteriaOperator.Parse("[Style] " +
                        "IN (" + string.Join(",", param?.ItemStyles.Select(x => "'" + x.CustomerStyle + "'")) + ")");
                    var itemModels = ObjectSpace.GetObjects<ItemModel>(criteria).ToList();

                    criteria = CriteriaOperator.Parse("[ItemStyleNumber] " +
                        "IN (" + string.Join(",", param?.ItemStyles.Select(x => "'" + x.Number + "'")) + ")");
                    var barCodes = ObjectSpace.GetObjects<ItemStyleBarCode>(criteria).ToList();

                    var newBoxGroups = IFG_CreateBoxGroupProcess
                            .CreateScanBarcode(ref param, objectSpaceBoxGroup, itemModels, barCodes, sheetName, ref errorMsg);
                    if (newBoxGroups != null)
                    {
                        /// Set box group -> IsPulled = true when create barcode on packing list
                        var updateBoxGroups = objectSpaceBoxGroup.GetObjects<BoxGroup>()
                            .Where(x => x.PackingListCode == param.PackingListCode &&
                                   x.IsPulled != true && x.CustomerID == param.CustomerID).ToList();
                        if (updateBoxGroups.Any())
                        {
                            updateBoxGroups.ForEach(x =>
                            {
                                x.IsPulled = true;
                            });
                        }

                        param.BarCodeCompleted = true;
                        objectSpaceBoxGroup.CommitChanges();
                    }
                }

                e.PopupWindowView.ObjectSpace.CommitChanges();
                if (packingLists == null)
                    packingLists = new List<PackingList>();
                packingLists.Add(param);
            }

            (View as ListView).EditView?.Refresh(true);
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
