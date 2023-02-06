using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Process;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class RevisedPackingList_ViewController : ObjectViewController<ListView, PackingList>
    {
        public RevisedPackingList_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction popupRevisedPackingList = new PopupWindowShowAction(this, "RevisedPackingList", PredefinedCategory.Unspecified);
            popupRevisedPackingList.ImageName = "Edit";
            popupRevisedPackingList.Caption = "Revised (Ctrl + R)";
            popupRevisedPackingList.TargetObjectType = typeof(PackingList);
            popupRevisedPackingList.TargetViewType = ViewType.ListView;
            popupRevisedPackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            popupRevisedPackingList.Shortcut = "CtrlR";

            popupRevisedPackingList.CustomizePopupWindowParams += PopupRevisedPackingList_CustomizePopupWindowParams;
            popupRevisedPackingList.Execute += PopupRevisedPackingList_Execute;
        }
        private void PopupRevisedPackingList_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace(typeof(PackingList));
            var packingListID = ((PackingList)View.CurrentObject).ID;
            var model = objectSpace.GetObjectByKey<PackingList>(packingListID);

            var view = Application.CreateDetailView(objectSpace, model, true);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = view;
        }
        private void PopupRevisedPackingList_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var param = e.PopupWindowViewCurrentObject as PackingList;
            var errorMessage = "";

            if (param.PackingLines.Any() && param.TotalQuantity > 0)
            {
                var prePack = param.PackingLines.ToList().First().PrePack.Trim();
                if (prePack == "Assorted Size - Solid Color")
                {
                    if (param.TotalQuantity != param.PackingLines.Sum(x => x.QuantitySize * x.TotalCarton))
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

            if (!string.IsNullOrEmpty(errorMessage.Trim()))
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
                    if (param.CustomerID == "IFG")
                    {
                        //param.ItemStyles.First().PackingOverQuantities.ToList().ForEach(x =>
                        foreach(var itemStyle in param.ItemStyles)
                        {
                            //param.ItemStyles.PackingOverQuantities.ToList().ForEach(x =>
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
                    else if (param.CustomerID == "PU")
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

                ///  Update IsRevised for Separated Packing List
                if (param.CustomerID == "IFG" || param.CustomerID == "GA")
                {
                    if (param.IsSeparated == true)
                    {
                        var tableName = "";
                        var filter = "";
                        var separatePackinglists = new List<PackingList>();
                        separatePackinglists.Add(param);

                        if (string.IsNullOrEmpty(tableName))
                            tableName = typeof(PackingList).Name;

                        var connectString = Application.ConnectionString ?? string.Empty;
                        using (var db = new QueryFactory(
                            new SqlConnection(connectString), new SqlServerCompiler()))
                        {
                            while (separatePackinglists.Any())
                            {
                                //filter = String.Format("[ParentPackingListID] IN {0}", separatePackinglists?.FirstOrDefault()?.ID);
                                filter = "[ParentPackingListID] IN (" + string.Join(",", separatePackinglists?.Select(x => x.ID)) + ")";

                                separatePackinglists = db.Query(tableName)
                                        .WhereRaw(filter).Get<PackingList>().ToList();

                                if (separatePackinglists.Any())
                                {
                                    var update = db.Query(tableName)
                                        .WhereIn("ID", separatePackinglists.Select(x => x.ID))
                                        .Update(new
                                        {
                                            IsRevised = true
                                        });
                                }
                            }
                            var originUpdate = db.Query(tableName)
                                .Where("ID", param.ID)
                                .Update(new
                                {
                                    IsRevised = true
                                });
                        }
                    }
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
