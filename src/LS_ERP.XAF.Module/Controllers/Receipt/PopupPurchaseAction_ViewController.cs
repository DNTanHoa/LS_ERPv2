using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Extensions;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class PopupPurchaseAction_ViewController : ViewController
    {
        public PopupPurchaseAction_ViewController()
        {
            InitializeComponent();

            SimpleAction loadPurchaseInforData = new SimpleAction(this, "LoadPurchaseInforData", PredefinedCategory.Unspecified);
            loadPurchaseInforData.ImageName = "Actions_Reload";
            loadPurchaseInforData.Caption = "Load Order";
            loadPurchaseInforData.TargetObjectType = typeof(PurchasePopupModel);
            loadPurchaseInforData.TargetViewType = ViewType.DetailView;
            loadPurchaseInforData.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            loadPurchaseInforData.Execute += LoadPurchaseInforData_Execute;

            SimpleAction copyEntryQuantityFromPurchaseQuantity = new SimpleAction(this, "CopyEntryQuantityFromPurchaseQuantity", PredefinedCategory.Unspecified);
            copyEntryQuantityFromPurchaseQuantity.ImageName = "Paste";
            copyEntryQuantityFromPurchaseQuantity.Caption = "Copy Quantity";
            copyEntryQuantityFromPurchaseQuantity.TargetObjectType = typeof(PurchasePopupModel);
            copyEntryQuantityFromPurchaseQuantity.TargetViewType = ViewType.DetailView;
            copyEntryQuantityFromPurchaseQuantity.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            copyEntryQuantityFromPurchaseQuantity.Execute += CopyEntryQuantityFromPurchaseQuantity_Execute;

            SimpleAction updatePurchaseDocument = new SimpleAction(this, "UpdatePurchaseDocument", PredefinedCategory.Unspecified);
            updatePurchaseDocument.ImageName = "UpdateTableOfContents";
            updatePurchaseDocument.Caption = "Update Document";
            updatePurchaseDocument.TargetObjectType = typeof(PurchasePopupModel);
            updatePurchaseDocument.TargetViewType = ViewType.DetailView;
            updatePurchaseDocument.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            updatePurchaseDocument.Execute += UpdatePurchaseDocument_Execute;

            SimpleAction deletePurchaseInforData = new SimpleAction(this, "DeletePurchaseInforData", PredefinedCategory.Unspecified);
            deletePurchaseInforData.ImageName = "Delete";
            deletePurchaseInforData.Caption = "Delete";
            deletePurchaseInforData.TargetObjectType = typeof(PurchaseOrderInforData);
            deletePurchaseInforData.TargetViewType = ViewType.ListView;
            deletePurchaseInforData.PaintStyle = ActionItemPaintStyle.Image;

            deletePurchaseInforData.Execute += DeletePurchaseInforData_Execute;

            SimpleAction dividePurchaseQuantityUp = new SimpleAction(this, "DividePurchaseQuantityUp", PredefinedCategory.Unspecified);
            dividePurchaseQuantityUp.ImageName = "UpdateTableOfContents";
            dividePurchaseQuantityUp.Caption = "Divide";
            dividePurchaseQuantityUp.TargetObjectType = typeof(PurchasePopupModel);
            dividePurchaseQuantityUp.TargetViewType = ViewType.DetailView;
            dividePurchaseQuantityUp.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            dividePurchaseQuantityUp.Execute += DividePurchaseQuantityUp_Execute;

            SimpleAction copyEntryQuantityFromPurchaseQuantityGroup = new SimpleAction(this, "CopyEntryQuantityFromPurchaseQuantityGroup", PredefinedCategory.Unspecified);
            copyEntryQuantityFromPurchaseQuantityGroup.ImageName = "Paste";
            copyEntryQuantityFromPurchaseQuantityGroup.Caption = "Copy Group Quantity";
            copyEntryQuantityFromPurchaseQuantityGroup.TargetObjectType = typeof(PurchasePopupModel);
            copyEntryQuantityFromPurchaseQuantityGroup.TargetViewType = ViewType.DetailView;
            copyEntryQuantityFromPurchaseQuantityGroup.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            copyEntryQuantityFromPurchaseQuantityGroup.Execute +=
                CopyEntryQuantityFromPurchaseQuantityGroup_Execute;

            SimpleAction groupPurchaseByContract = new SimpleAction(this, "GroupPurchaseByContract", PredefinedCategory.Unspecified);
            groupPurchaseByContract.ImageName = "Paste";
            groupPurchaseByContract.Caption = "Group Contract";
            groupPurchaseByContract.TargetObjectType = typeof(PurchasePopupModel);
            groupPurchaseByContract.TargetViewType = ViewType.DetailView;
            groupPurchaseByContract.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            groupPurchaseByContract.Execute += GroupPurchaseByContract_Execute;

            SimpleAction groupPurchaseByGarmentSize = new SimpleAction(this, "GroupPurchaseByGarmentSize", PredefinedCategory.Unspecified);
            groupPurchaseByGarmentSize.ImageName = "Paste";
            groupPurchaseByGarmentSize.Caption = "Group Size";
            groupPurchaseByGarmentSize.TargetObjectType = typeof(PurchasePopupModel);
            groupPurchaseByGarmentSize.TargetViewType = ViewType.DetailView;
            groupPurchaseByGarmentSize.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            groupPurchaseByGarmentSize.Execute += GroupPurchaseByGarmentSize_Execute;

            SimpleAction groupPurchaseByGarmentColor = new SimpleAction(this, "GroupPurchaseByGarmentColor", PredefinedCategory.Unspecified);
            groupPurchaseByGarmentColor.ImageName = "Paste";
            groupPurchaseByGarmentColor.Caption = "Group Color";
            groupPurchaseByGarmentColor.TargetObjectType = typeof(PurchasePopupModel);
            groupPurchaseByGarmentColor.TargetViewType = ViewType.DetailView;
            groupPurchaseByGarmentColor.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            groupPurchaseByGarmentColor.Execute += GroupPurchaseByGarmentColor_Execute;

        }

        private void GroupPurchaseByGarmentColor_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PurchasePopupModel;

            if (viewObject != null &&
                viewObject.PurchaseInfor.Any())
            {
                var purchaseInforsCache = viewObject.PurchaseInfor;

                var purchaseGroups = purchaseInforsCache
                .GroupBy(x => new
                {
                    x.ItemID,
                    x.DsmItemID,
                    x.ItemName,
                    x.ItemColorCode,
                    x.ItemColorName,
                    x.Specify,
                    x.ContractNo,
                    x.GarmentSize,
                    x.Season,
                    x.PurchaseUnitID,
                    x.EntryUnitID,
                })
                .Select(x => new PurchaseOrderGroupInforData()
                {
                    ItemID = x.Key.ItemID,
                    DsmItemID = x.Key.DsmItemID,
                    ItemName = x.Key.ItemName,
                    ItemColorCode = x.Key.ItemColorCode,
                    ItemColorName = x.Key.ItemColorName,
                    Specify = x.Key.Specify,
                    ContractNo = x.Key.ContractNo,
                    Season = x.Key.Season,
                    PurchaseUnitID = x.Key.PurchaseUnitID,
                    EntryUnitID = x.Key.EntryUnitID,
                    GarmentSize = x.Key.GarmentSize,
                    PurchaseQuantity = x.Sum(i => i.PurchaseQuantity)
                });

                viewObject.PurchaseGroup = purchaseGroups.ToList();
                View.Refresh();
            }
        }

        private void GroupPurchaseByGarmentSize_Execute(object sender,
            SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PurchasePopupModel;

            if (viewObject != null &&
                viewObject.PurchaseInfor.Any())
            {
                var purchaseInforsCache = viewObject.PurchaseInfor;

                var purchaseGroups = purchaseInforsCache
                .GroupBy(x => new
                {
                    x.ItemID,
                    x.DsmItemID,
                    x.ItemName,
                    x.ItemColorCode,
                    x.ItemColorName,
                    x.Specify,
                    x.ContractNo,
                    x.GarmentColorCode,
                    x.GarmentColorName,
                    x.Season,
                    x.PurchaseUnitID,
                    x.EntryUnitID,
                })
                .Select(x => new PurchaseOrderGroupInforData()
                {
                    ItemID = x.Key.ItemID,
                    DsmItemID = x.Key.DsmItemID,
                    ItemName = x.Key.ItemName,
                    ItemColorCode = x.Key.ItemColorCode,
                    ItemColorName = x.Key.ItemColorName,
                    Specify = x.Key.Specify,
                    ContractNo = x.Key.ContractNo,
                    Season = x.Key.Season,
                    PurchaseUnitID = x.Key.PurchaseUnitID,
                    EntryUnitID = x.Key.EntryUnitID,
                    GarmentColorCode = x.Key.GarmentColorCode,
                    GarmentColorName = x.Key.GarmentColorName,
                    PurchaseQuantity = x.Sum(i => i.PurchaseQuantity)
                });

                viewObject.PurchaseGroup = purchaseGroups.ToList();
                View.Refresh();
            }
        }

        private void GroupPurchaseByContract_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PurchasePopupModel;
            if (viewObject != null &&
               viewObject.PurchaseInfor.Any())
            {
                var purchaseInforsCache = viewObject.PurchaseInfor;
                viewObject.GetGroup = true;

                var purchaseGroups = purchaseInforsCache
                .GroupBy(x => new
                {
                    x.ItemID,
                    x.DsmItemID,
                    x.ItemName,
                    x.ItemColorCode,
                    x.ItemColorName,
                    x.Specify,
                    x.ContractNo,
                    x.Season,
                    x.PurchaseUnitID,
                    x.EntryUnitID
                })
                .Select(x => new PurchaseOrderGroupInforData()
                {
                    ItemID = x.Key.ItemID,
                    DsmItemID = x.Key.DsmItemID,
                    ItemName = x.Key.ItemName,
                    ItemColorCode = x.Key.ItemColorCode,
                    ItemColorName = x.Key.ItemColorName,
                    Specify = x.Key.Specify,
                    ContractNo = x.Key.ContractNo,
                    Season = x.Key.Season,
                    PurchaseUnitID = x.Key.PurchaseUnitID,
                    EntryUnitID = x.Key.EntryUnitID,
                    PurchaseQuantity = x.Sum(i => i.PurchaseQuantity)
                });

                viewObject.PurchaseGroup = purchaseGroups.ToList();
                View.Refresh();
            }
        }

        private void CopyEntryQuantityFromPurchaseQuantityGroup_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PurchasePopupModel;

            if (viewObject != null &&
                viewObject.PurchaseGroup.Any())
            {
                ListPropertyEditor listPropertyEditor = ((DetailView)View)
                    .FindItem("PurchaseGroup") as ListPropertyEditor;

                var groupCopied = listPropertyEditor.ListView.SelectedObjects
                    .Cast<PurchaseOrderGroupInforData>();

                if (groupCopied != null &&
                    groupCopied.Any())
                {
                    foreach (var group in groupCopied)
                    {
                        group.EntryQuantity = group.PurchaseQuantity;
                    }
                }
            }

            View.Refresh();
        }

        private void DividePurchaseQuantityUp_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PurchasePopupModel;

            if (viewObject != null &&
               viewObject.PurchaseGroup.Any())
            {
                foreach (var group in viewObject.PurchaseGroup)
                {
                    if (group.EntryQuantity != null && group.EntryQuantity > 0)
                    {
                        var purchaseInfors = viewObject.PurchaseInfor
                        .Where(x => x.ItemID == group.ItemID &&
                                    x.ItemName == group.ItemName &&
                                    x.ItemColorCode == group.ItemColorCode &&
                                    x.ItemColorName == group.ItemColorName &&
                                    x.Specify == group.Specify &&
                                    x.CustomerStyle == group.CustomerStyle &&
                                    x.Season == group.Season &&
                                    (x.GarmentSize == group.GarmentSize || string.IsNullOrEmpty(group.GarmentSize)) &&
                                    (x.GarmentColorCode == group.GarmentColorCode || string.IsNullOrEmpty(group.GarmentColorCode)));

                        if (purchaseInfors != null)
                        {
                            var groupQuantity = group.EntryQuantity;
                            foreach (var purchaseInfor in purchaseInfors)
                            {
                                var purchaseQuantity = purchaseInfor.PurchaseQuantity 
                                    - (purchaseInfor.ReceiptQuantity ?? 0);

                                if (groupQuantity > purchaseQuantity)
                                {
                                    if (purchaseInfor.Equals(purchaseInfors.Last()))
                                    {
                                        purchaseInfor.EntryQuantity = groupQuantity;
                                    }
                                    else
                                    {
                                        purchaseInfor.EntryQuantity = purchaseQuantity;
                                    }
                                }
                                else
                                {
                                    purchaseInfor.EntryQuantity = groupQuantity;
                                }

                                groupQuantity -= (decimal)purchaseInfor.EntryQuantity;
                            }
                        }
                    }
                }
            }

            View.Refresh();
        }

        private void DeletePurchaseInforData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UpdatePurchaseDocument_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PurchasePopupModel;

            if (viewObject != null &&
               viewObject.PurchaseInfor.Any())
            {
                foreach (var infor in viewObject.PurchaseInfor)
                {
                    infor.LotNumber = viewObject.LotNumber;
                    infor.BinCode = viewObject.BinCode;
                }
            }

            if (viewObject != null &&
               viewObject.PurchaseGroup.Any())
            {
                foreach (var group in viewObject.PurchaseGroup)
                {
                    group.LotNumber = viewObject.LotNumber;
                    group.BinCode = viewObject.BinCode;
                }
            }

            View.Refresh();
        }

        private void CopyEntryQuantityFromPurchaseQuantity_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PurchasePopupModel;

            if (viewObject != null &&
               viewObject.PurchaseInfor.Any())
            {
                ListPropertyEditor listPropertyEditor = ((DetailView)View)
                    .FindItem("PurchaseInfor") as ListPropertyEditor;

                var copiedInfor = listPropertyEditor.ListView.SelectedObjects.Cast<PurchaseOrderInforData>().ToList();

                if (copiedInfor != null &&
                   copiedInfor.Any())
                {
                    foreach (var infor in copiedInfor)
                    {
                        if (infor.ReceiptQuantity != null)
                        {
                            infor.EntryQuantity = infor.RemainQuantity;
                        }
                        else
                        {
                            infor.EntryQuantity = infor.PurchaseQuantity;
                        }
                    }
                }
            }

            View.Refresh();
        }

        private void LoadPurchaseInforData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PurchasePopupModel;
            var criteria = CriteriaOperator.Parse("PurchaseOrderID = ? AND (ReceiptQuantity < Quantity OR ReceiptQuantity Is Null)", viewObject.PurchaseOrder?.ID);
            var purchaseGroupLines = ObjectSpace.GetObjects<PurchaseOrderGroupLine>(criteria);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PurchaseOrderGroupLine, PurchaseOrderInforData>()
                    .ForMember(x => x.PurchaseUnitID, y => y.MapFrom(s => s.WareHouseUnitID))
                    .ForMember(x => x.EntryUnitID, y => y.MapFrom(s => s.WareHouseUnitID))
                    .ForMember(x => x.PurchaseOrderGroupLineID, y => y.MapFrom(s => s.ID))
                    .ForMember(x => x.PurchaseQuantity, y => y.MapFrom(s => s.RoundQuantity()))
                    .ForMember(x => x.DetailInforDatas, y => y.MapFrom(s => s.PurchaseOrderLines))
                    .ForMember(x => x.EntryQuantity, y => y.MapFrom(s => 0));

                cfg.CreateMap<PurchaseOrderLine, PurchaseOrderDetailInforData>()
                    .ForMember(x => x.PurchaseUnitID, y => y.MapFrom(s => s.UnitID))
                    .ForMember(x => x.EntryUnitID, y => y.MapFrom(s => s.UnitID))
                    .ForMember(x => x.PurchaseQuantity, y => y.MapFrom(s => s.WareHouseQuantity))
                    .ForMember(x => x.EntryQuantity, y => y.MapFrom(s => 0))
                    .ForMember(x => x.PurchaseOrderLineID, y => y.MapFrom(s => s.ID));
            });

            var mapper = config.CreateMapper();

            var purchaseInfors = new List<PurchaseOrderInforData>();

            foreach (var purchaseGroupLine in purchaseGroupLines)
            {
                purchaseGroupLine.Quantity = purchaseGroupLine.RoundQuantity();
                purchaseGroupLine.ItemName = purchaseGroupLine.ItemName?
                    .Replace("\r\n", "").Replace("\n", "");
                purchaseGroupLine.ItemColorName = purchaseGroupLine.ItemColorName?
                    .Replace("\r\n", "").Replace("\n", "");

                var infor = mapper.Map<PurchaseOrderInforData>(purchaseGroupLine);

                purchaseInfors.Add(infor);
            }

            var purchaseInforsCache = purchaseInfors.ToList();

            switch (viewObject.PurchaseOrder?.CustomerID)
            {
                case "GA":
                    {
                        if (viewObject.Unit == null)
                        {
                            viewObject.Unit = ObjectSpace.GetObjectByKey<Unit>(purchaseInfors.FirstOrDefault()?.PurchaseUnitID);
                        }
                        else
                        {
                            foreach (var item in purchaseInforsCache)
                            {
                                item.EntryUnitID = viewObject.Unit?.ID;
                            }
                        }
                    }
                    break;
                default:
                    {
                        //if (viewObject.Unit == null)
                        //{
                        //    viewObject.Unit = ObjectSpace.GetObjectByKey<Unit>(purchaseInfors.FirstOrDefault()?.PurchaseUnitID);
                        //}
                    }
                    break;
            }


            var purchaseGroups = purchaseInforsCache
                .GroupBy(x => new
                {
                    x.ItemID,
                    x.DsmItemID,
                    x.ItemName,
                    x.ItemColorCode,
                    x.ItemColorName,
                    x.Specify,
                    x.CustomerStyle,
                    x.Season,
                    x.PurchaseUnitID,
                    x.EntryUnitID
                })
                .Select(x => new PurchaseOrderGroupInforData()
                {
                    ItemID = x.Key.ItemID,
                    DsmItemID = x.Key.DsmItemID,
                    ItemName = x.Key.ItemName,
                    ItemColorCode = x.Key.ItemColorCode,
                    ItemColorName = x.Key.ItemColorName,
                    Specify = x.Key.Specify,
                    CustomerStyle = x.Key.CustomerStyle,
                    Season = x.Key.Season,
                    PurchaseUnitID = x.Key.PurchaseUnitID,
                    EntryUnitID = x.Key.EntryUnitID,
                    PurchaseQuantity = x.Sum(i => i.PurchaseQuantity)
                });

            viewObject.PurchaseInfor = purchaseInfors;
            viewObject.PurchaseGroup = purchaseGroups.ToList();

            View.Refresh();

            if (!purchaseInfors.Any())
            {
                var infor = Message.GetMessageOptions("Purchase has no data to entry", "infor",
                    InformationType.Info, null, 2000);
                Application.ShowViewStrategy.ShowMessage(infor);
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
