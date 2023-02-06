using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class MaterialRequestDetail_ViewController : ObjectViewController<ListView, MaterialRequestDetail>
    {
        public MaterialRequestDetail_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction getProductionBomForMaterialRequest = new PopupWindowShowAction(this, "GetProductionBomForMaterialRequest",
                PredefinedCategory.Unspecified);

            getProductionBomForMaterialRequest.ImageName = "Header";
            getProductionBomForMaterialRequest.Caption = "Production Bom";
            getProductionBomForMaterialRequest.TargetObjectType = typeof(MaterialRequestDetail);
            getProductionBomForMaterialRequest.TargetViewType = ViewType.ListView;
            getProductionBomForMaterialRequest.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            getProductionBomForMaterialRequest.Shortcut = "CtrlShiftB";

            getProductionBomForMaterialRequest.CustomizePopupWindowParams += GetProductionBomForMaterialRequest_CustomizePopupWindowParams;
            getProductionBomForMaterialRequest.Execute += GetProductionBomForMaterialRequest_Execute;

            PopupWindowShowAction editMaterialRequestLine = new PopupWindowShowAction(this, "EditMaterialRequestLine",
                PredefinedCategory.Unspecified);

            editMaterialRequestLine.ImageName = "TrackingChanges_TrackChanges";
            editMaterialRequestLine.Caption = "Edit Line";
            editMaterialRequestLine.TargetObjectType = typeof(MaterialRequestDetail);
            editMaterialRequestLine.TargetViewType = ViewType.ListView;
            editMaterialRequestLine.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            editMaterialRequestLine.Shortcut = "CtrlShiftB";

            editMaterialRequestLine.CustomizePopupWindowParams += EditMaterialRequestLine_CustomizePopupWindowParams;
            editMaterialRequestLine.Execute += EditMaterialRequestLine_Execute;

            SimpleAction copyQuantityInMaterialRequestLine = new SimpleAction(this, "CopyQuantityInMaterialRequestLine", PredefinedCategory.Unspecified);
            copyQuantityInMaterialRequestLine.ImageName = "Paste";
            copyQuantityInMaterialRequestLine.Caption = "Copy Quantity";
            copyQuantityInMaterialRequestLine.TargetObjectType = typeof(MaterialRequestDetail);
            copyQuantityInMaterialRequestLine.TargetViewType = ViewType.ListView;
            copyQuantityInMaterialRequestLine.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            copyQuantityInMaterialRequestLine.Execute += CopyQuantityInMaterialRequestLine_Execute;

            SimpleAction calculateRequestQuantity = new SimpleAction(this, "CalculateRequestQuantity", PredefinedCategory.Unspecified);
            calculateRequestQuantity.ImageName = "RenameDataSource";
            calculateRequestQuantity.Caption = "Cal. Quantity";
            calculateRequestQuantity.TargetObjectType = typeof(MaterialRequestDetail);
            calculateRequestQuantity.TargetViewType = ViewType.ListView;
            calculateRequestQuantity.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            calculateRequestQuantity.Execute += CalculateRequestQuantity_Execute;
        }

        private void CalculateRequestQuantity_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var requestDetails = View.SelectedObjects.Cast<MaterialRequestDetail>();
            
            foreach(var requestDetail in requestDetails)
            {
                var requestOrder = requestDetail.MaterialRequest
                    .OrderDetails.FirstOrDefault(x => x.LSStyle == requestDetail.LSStyle &&
                                                      x.GarmentSize.Replace(" ", "").ToUpper().Trim() == requestDetail.GarmentSize.Replace(" ", "").ToUpper().Trim());
                if(requestOrder != null)
                {
                    var orderQuantity = requestOrder.RequestQuantity + 
                            requestOrder.SampleQuantity + 
                            requestOrder.PercentQuantity;

                    requestDetail.Quantity = requestDetail.QuantityPerUnit * orderQuantity; 
                }
            }

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
            View.Refresh();
        }

        private void CopyQuantityInMaterialRequestLine_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var requestDetails = View.SelectedObjects.Cast<MaterialRequestDetail>();
            
            foreach(var requestDetail in requestDetails)
            {
                requestDetail.Quantity = requestDetail.RequiredQuantity;
            }

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
            View.Refresh();
        }

        private void EditMaterialRequestLine_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowViewCurrentObject as MaterialRequestDetailEdit;
            var materialRequest = (((ListView)View).CollectionSource as PropertyCollectionSource).MasterObject
                as MaterialRequest;

            if (viewObject != null)
            {
                foreach(var group in viewObject.Groups)
                {
                    var inputQuantity = group.Quantity;

                    foreach(var detail in group.Details)
                    {
                        var line = materialRequest.Details.FirstOrDefault(x => x.Id == detail.Id);

                        if (line == null)
                            continue;

                        if(inputQuantity >= line.RequiredQuantity)
                        {
                            line.Quantity = line.RequiredQuantity;
                        }
                        else if(inputQuantity < line.RequiredQuantity &&
                                inputQuantity > 0)
                        {
                            line.Quantity = inputQuantity;
                        }
                        else
                        {
                            line.Quantity = 0;
                        }

                        if(line.Id == group.Details.Last().Id)
                        {
                            line.Quantity = inputQuantity;
                        }

                        inputQuantity -= line.Quantity;
                    }
                }

                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
                View.Refresh();
            }
        }

        private void EditMaterialRequestLine_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var request = (((ListView)View).CollectionSource as PropertyCollectionSource).MasterObject
                as MaterialRequest;

            if (request != null)
            {
                var groupSizeLines = request.Details.Where(x => x.GroupSize == true &&
                                                                x.GroupItemColor != true)
                    .GroupBy(x => new
                    {
                        x.ItemName,
                        x.ItemColorCode,
                        x.ItemColorName,
                        x.Season,
                        x.Specify,
                        x.CustomerStyle,
                        x.GarmentColorCode,
                        x.GarmentColorName
                    }).Select(x => new MaterialRequestDetailGroup()
                    {
                        ItemName = x.Key.ItemName,
                        ItemColorCode = x.Key.ItemColorCode,
                        ItemColorName = x.Key.ItemColorName,
                        Specify = x.Key.Specify,
                        GarmentColorCode = x.Key.GarmentColorCode,
                        GarmentColorName = x.Key.GarmentColorName,
                        RequiredQuantity = x.Sum(x => x.RequiredQuantity),
                        Quantity = x.Sum(x => x.Quantity),
                        QuantityPerUnit = x.First().QuantityPerUnit,
                        Remarks = x.First().Remarks,
                        Details = x.Select(x => x).ToList()
                    }).ToList();

                var groupItemColorLines = request.Details.Where(x => x.GroupItemColor == true)
                    .GroupBy(x => new
                    {
                        x.ItemName,
                        x.Season,
                        x.Specify,
                        x.CustomerStyle,
                        x.GarmentColorCode,
                        x.GarmentColorName
                    }).Select(x => new MaterialRequestDetailGroup()
                    {
                        ItemName = x.Key.ItemName,
                        Specify = x.Key.Specify,
                        GarmentColorCode = x.Key.GarmentColorCode,
                        GarmentColorName = x.Key.GarmentColorName,
                        RequiredQuantity = x.Sum(x => x.RequiredQuantity),
                        Quantity = x.Sum(x => x.Quantity),
                        QuantityPerUnit = x.First().QuantityPerUnit,
                        Remarks = x.First().Remarks,
                        Details = x.Select(x => x).ToList()
                    })
                    .ToList();

                var groupNormalLines = request.Details
                    .Where(x => x.GroupItemColor != true &&
                                x.GroupSize != true)
                    .GroupBy(x => new
                    {
                        x.ItemName,
                        x.ItemColorCode,
                        x.ItemColorName,
                        x.Season,
                        x.Specify,
                        x.CustomerStyle,
                        x.GarmentColorCode,
                        x.GarmentColorName,
                        x.GarmentSize
                    }).Select(x => new MaterialRequestDetailGroup()
                    {
                        ItemName = x.Key.ItemName,
                        ItemColorCode = x.Key.ItemColorCode,
                        ItemColorName = x.Key.ItemColorName,
                        Specify = x.Key.Specify,
                        GarmentColorCode = x.Key.GarmentColorCode,
                        GarmentColorName = x.Key.GarmentColorName,
                        RequiredQuantity = x.Sum(x => x.RequiredQuantity),
                        Quantity = x.Sum(x => x.Quantity),
                        QuantityPerUnit = x.First().QuantityPerUnit,
                        Remarks = x.First().Remarks,
                        GarmentSize = x.Key.GarmentSize,
                        Details = x.Select(x => x).ToList()
                    }).ToList();

                var groups = new List<MaterialRequestDetailGroup>();
                groups.AddRange(groupSizeLines);
                groups.AddRange(groupItemColorLines);
                groups.AddRange(groupNormalLines);
                
                var model = new MaterialRequestDetailEdit()
                {
                    Groups = groups,
                };

                var objectSpace = this.ObjectSpace;
                e.DialogController.SaveOnAccept = false;
                e.Maximized = true;
                var view = Application.CreateDetailView(objectSpace, model, false);
                e.View = view;
            }
        }

        private void GetProductionBomForMaterialRequest_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowViewCurrentObject as MaterialRequestFromProductionBom;
            var materialRequest = (((ListView)View).CollectionSource as PropertyCollectionSource).MasterObject
                as MaterialRequest;

            if(viewObject != null)
            {
                ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
                    .FindItem("MaterialRequestDetailPreviews") as ListPropertyEditor;

                if(listPropertyEditor != null)
                {
                    var selectedProBoms = listPropertyEditor.ListView
                        .SelectedObjects.Cast<MaterialRequestDetailPreview>().ToList();

                    var materialRequestDetails = selectedProBoms
                        .Select(x => new MaterialRequestDetail()
                        {
                            ItemCode = x.ItemCode,
                            ItemID = x.ItemID,
                            ItemName = x.ItemName,
                            ItemColorCode = x.ItemColorCode,
                            ItemColorName = x.ItemColorName,
                            Specify = x.Specify,
                            GarmentColorCode = x.GarmentColorCode,
                            GarmentColorName = x.GarmentColorName,
                            GarmentSize = x.GarmentSize,
                            UnitID = x.PriceUnitID,
                            CustomerStyle = x.CustomerStyle,
                            LSStyle = x.LSStyle,
                            Season = x.Season,
                            RequiredQuantity = x.RequiredQuantity ?? 0,
                            QuantityPerUnit = x.QuantityPerUnit ?? 0,
                            GroupSize = x.GroupSize,
                            GroupItemColor = x.GroupItemColor,
                            Position = x.Position,
                            OtherName = x.OtherName
                        }).ToList();

                    materialRequest.Details.AddRange(materialRequestDetails);

                    if (string.IsNullOrEmpty(materialRequest.CreatedBy))
                    {
                        materialRequest.SetCreateAudit(SecuritySystem.CurrentUserName);
                    }
                    else
                    {
                        materialRequest.SetUpdateAudit(SecuritySystem.CurrentUserName);
                    }

                    var styles = string.Join(";", materialRequest.Details.Select(x => x.LSStyle).Distinct());
                    materialRequest.LSStyles = styles;

                    ObjectSpace.CommitChanges();
                }

                ListPropertyEditor itemStylePropertyEditor = ((DetailView)e.PopupWindowView)
                   .FindItem("ItemStyles") as ListPropertyEditor;

                if(itemStylePropertyEditor != null)
                {
                    var selectedItemStyles = itemStylePropertyEditor.ListView
                        .SelectedObjects.Cast<ItemStyle>().ToList();

                    foreach(var itemStyle in selectedItemStyles)
                    {
                        foreach(var orderDetail in itemStyle.OrderDetails)
                        {
                            var materialRequestOrderDetail = ObjectSpace
                                .CreateObject<MaterialRequestOrderDetail>();

                            materialRequestOrderDetail.ItemStyleNumber = orderDetail.ItemStyleNumber;
                            materialRequestOrderDetail.OrderDetailID = (int)orderDetail.ID;
                            materialRequestOrderDetail.MaterialRequestId = materialRequest.Id;
                            materialRequestOrderDetail.CustomerStyle = itemStyle.CustomerStyle;
                            materialRequestOrderDetail.LSStyle = itemStyle.LSStyle;
                            materialRequestOrderDetail.ColorCode = itemStyle.ColorCode;
                            materialRequestOrderDetail.ColorName = itemStyle.ColorName;
                            materialRequestOrderDetail.GarmentSize = orderDetail.Size;
                            materialRequestOrderDetail.SizeSortIndex = orderDetail.SizeSortIndex ?? 0;
                            materialRequestOrderDetail.Quantity = (int)(orderDetail.Quantity ?? 0);
                            materialRequestOrderDetail.RequestQuantity = (int)(orderDetail.Quantity ?? 0);
                            materialRequestOrderDetail.SampleQuantity = (int)(orderDetail.SampleQuantity);
                            materialRequestOrderDetail.PercentQuantity = (int)(orderDetail.OverPercent);
                        }
                    }
                }

                ObjectSpace.CommitChanges();
                View.Refresh();
                ObjectSpace.Refresh();
            }
        }

        private void GetProductionBomForMaterialRequest_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            var model = objectSpace.CreateObject<MaterialRequestFromProductionBom>();
            var view = Application.CreateDetailView(objectSpace, model, false);
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
