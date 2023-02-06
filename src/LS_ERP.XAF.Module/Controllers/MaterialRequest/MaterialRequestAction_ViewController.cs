using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using DevExpress.XtraReports.UI;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Report;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class MaterialRequestAction_ViewController : ViewController
    {
        public MaterialRequestAction_ViewController()
        {
            InitializeComponent();

            SimpleAction processMaterialRequestAction = new SimpleAction(this, "ProcessMaterialRequestAction", PredefinedCategory.Unspecified);
            processMaterialRequestAction.ImageName = "MainMenuItem";
            processMaterialRequestAction.Caption = "Process (Shift + P)";
            processMaterialRequestAction.TargetObjectType = typeof(MaterialRequest);
            processMaterialRequestAction.TargetViewType = ViewType.Any;
            processMaterialRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            processMaterialRequestAction.Execute += ProcessMaterialRequestAction_Execute;

            PopupWindowShowAction editOrderQuantityAction = new PopupWindowShowAction(this, "EditOrderQuantityAction", PredefinedCategory.Unspecified);
            editOrderQuantityAction.ImageName = "MainMenuItem";
            editOrderQuantityAction.Caption = "Edit Order Quantity";
            editOrderQuantityAction.TargetObjectType = typeof(MaterialRequest);
            editOrderQuantityAction.TargetViewType = ViewType.Any;
            editOrderQuantityAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            editOrderQuantityAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            editOrderQuantityAction.CustomizePopupWindowParams += EditOrderQuantityAction_CustomizePopupWindowParams;
            editOrderQuantityAction.Execute += EditOrderQuantityAction_Execute;

            SimpleAction printRequestAction = new SimpleAction(this, "PrintRequestAction", PredefinedCategory.Unspecified);
            printRequestAction.ImageName = "Action_Search_Object_FindObjectByID";
            printRequestAction.Caption = "Print (Shift + P)";
            printRequestAction.TargetObjectType = typeof(MaterialRequest);
            printRequestAction.TargetViewType = ViewType.Any;
            printRequestAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            printRequestAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            printRequestAction.Execute += PrintRequestAction_Execute;

            SimpleAction printRequestWithSizeBreakDownAction = new SimpleAction(this, "PrintRequestWithSizeBreakDownAction", PredefinedCategory.Unspecified);
            printRequestWithSizeBreakDownAction.ImageName = "Action_Search_Object_FindObjectByID";
            printRequestWithSizeBreakDownAction.Caption = "Print For Size";
            printRequestWithSizeBreakDownAction.TargetObjectType = typeof(MaterialRequest);
            printRequestWithSizeBreakDownAction.TargetViewType = ViewType.Any;
            printRequestWithSizeBreakDownAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            printRequestWithSizeBreakDownAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            printRequestWithSizeBreakDownAction.Execute += PrintRequestWithSizeBreakDownAction_Execute;
        }

        private void EditOrderQuantityAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ObjectSpace.CommitChanges();
            View.Refresh();
        }

        private void EditOrderQuantityAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace(typeof(OrderDetail));
            var materialRequest = View.CurrentObject as MaterialRequest;
            var dataSource = new List<MaterialRequestDetailModel>();
            var lsStyles = materialRequest.LSStyles.Split(";").Select(x => "'" + x + "'");
            var itemStylesCriteria = CriteriaOperator
                .Parse("LSStyle IN (" + string.Join(",", lsStyles) + ")");
            var sizeBreakDowns = objectSpace.GetObjects<ItemStyle>(itemStylesCriteria)
                                    .SelectMany(x => x.OrderDetails);

            var model = new MaterialRequestEditSizeQuantity()
            {
                OrderDetails = sizeBreakDowns.ToList()
            };
            e.Maximized = true;
            var view = Application.CreateDetailView(objectSpace, model);
            e.View = view;
        }

        private void PrintRequestWithSizeBreakDownAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var materialRequest = View.CurrentObject as MaterialRequest;
            var dataSource = new List<MaterialRequestDetailModel>();

            /// Get Size breakdown
            var groupSize = materialRequest.OrderDetails?.GroupBy(x => new
            {
                x.CustomerStyle,
                x.GarmentSize,
                x.SizeSortIndex
            })
            .OrderBy(x => x.Key?.SizeSortIndex)
            .Select(x => new SizeBreakDownModel()
            {
                CustomerStyle = x.Key.CustomerStyle,
                GarmentSize = x.Key.GarmentSize,
                Quantity = x.Sum(x => (x.RequestQuantity + x.PercentQuantity + x.SampleQuantity)),
                RequestQuantity = x.Sum(x => x.RequestQuantity),
                PercentQuantity = x.Sum(x => x.PercentQuantity),
                SampleQuantity = x.Sum(x => x.SampleQuantity),
                OrderQuantity = x.Sum(x => x.RequestQuantity)
            });

            /// Get item
            var detailGroups = materialRequest.Details.GroupBy(x => new
            {
                x.ItemName,
                x.ItemColorCode,
                x.ItemColorName,
                x.Specify,
                x.GroupSize
            });

            foreach (var detailGroup in detailGroups.OrderBy(x => x.Key.ItemName))
            {
                var dataSourceItem = new MaterialRequestDetailModel()
                {
                    ItemID = detailGroup.FirstOrDefault()?.ItemID,
                    OtherName = detailGroup.First().OtherName,
                    ItemName = $"{detailGroup.Key.ItemName}-{detailGroup.Key.ItemColorCode}-{detailGroup.Key.ItemColorName}" +
                    $"-{detailGroup.Key.Specify}",
                    ItemColorCode = detailGroup.Key.ItemColorName,
                    ItemColorName = detailGroup.Key.ItemColorName,
                    Quantity = Math.Round(detailGroup.Sum(x => x.Quantity)),
                    QuantityPerUnit = detailGroup.First().QuantityPerUnit,
                    UnitID = detailGroup.First().UnitID,
                    CustomerStyle = detailGroup.First().GarmentColorCode
                };

                dataSource.Add(dataSourceItem);
            }

            switch (materialRequest.StorageCode)
            {
                case "AC":
                    var report = new MaterialRequestSizeBreakDownReport();
                    report.DataSource = new List<MaterialRequestReportModel>()
                    {
                        new MaterialRequestReportModel 
                        { 
                            RequestDate = materialRequest.RequestDate,
                            RequestFor = materialRequest.RequestFor,
                            Remark = materialRequest.LSStyles,
                            CustomerStyle = materialRequest.Details
                                .FirstOrDefault()?.GarmentColorCode,
                        }
                    };

                    /// Sizes band
                    DetailReportBand sizeDetailBand = (DetailReportBand)report
                        .FindControl("DetailReport", false);
                    sizeDetailBand.DataSource = groupSize.ToList();
                    /// Item band
                    DetailReportBand itemDetailBand = (DetailReportBand)report
                        .FindControl("DetailReport1", false);
                    itemDetailBand.DataSource = dataSource;

                    ReportPrintTool materialRequestPrintTool = new ReportPrintTool(report);
                    materialRequestPrintTool.ShowPreview();
                    break;
                default:
                    break;
            }
        }

        private void PrintRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var materialRequest = View.CurrentObject as MaterialRequest;
            var dataSource = new List<MaterialRequestDetailModel>();

            var data = materialRequest.Details;
            data.ForEach(item =>
            {
                item.ItemColorCode = item.GroupItemColor != true ? item.ItemColorCode : string.Empty;
                item.ItemColorName = item.GroupItemColor != true ? item.ItemColorName : string.Empty;
            });

            if(materialRequest != null)
            {
                var detailGroups = materialRequest.Details.GroupBy(x => new
                {
                    x.ItemName,
                    x.ItemColorCode,
                    x.ItemColorName,
                    x.Specify,
                    x.GroupSize
                });

                foreach(var detailGroup in detailGroups.OrderBy(x => x.Key.ItemName))
                {
                    var dataSourceItem = new MaterialRequestDetailModel()
                    {
                        ItemID = String.Empty,
                        OtherName = detailGroup.First().OtherName,
                        ItemName = $"{detailGroup.Key.ItemName}-{detailGroup.Key.ItemColorCode}-{detailGroup.Key.ItemColorName}" +
                        $"-{detailGroup.Key.Specify}",
                        ItemColorCode = detailGroup.Key.ItemColorName,
                        ItemColorName = detailGroup.Key.ItemColorName,
                        Quantity = Math.Round(detailGroup.Sum(x => x.Quantity)),
                        QuantityPerUnit = detailGroup.First().QuantityPerUnit,
                        UnitID = detailGroup.First().UnitID,
                        CustomerStyle = detailGroup.First().CustomerStyle,
                        MaterialRequest = new MaterialRequestReportModel()
                        {
                            Id = detailGroup.First().MaterialRequest.Id,
                            CustomerStyle = detailGroup.First().GarmentColorCode,
                            RequestFor = detailGroup.First().MaterialRequest.RequestFor,
                            RequestDate = detailGroup.First().MaterialRequest.RequestDate,
                            Remark = string.Join(";", materialRequest.Details.Select(x => x.LSStyle).Distinct())
                        },
                        GarmentSize = detailGroup.Key.GroupSize != true ?
                            string.Join("\r\n", detailGroup.GroupBy(x => x.GarmentSize).Select(x => x.Key + $"({(int)x.Sum(e => e.Quantity)})")) : string.Empty,
                    };

                    dataSource.Add(dataSourceItem);
                }
            }

            switch(materialRequest.StorageCode)
            {

                case "AC":
                    var report = new MaterialRequestReport();
                    report.DataSource = dataSource.OrderBy(x => x.OtherName);
                    ReportPrintTool materialRequestPrintTool = new ReportPrintTool(report);
                    materialRequestPrintTool.ShowPreview();
                    break;
                default:
                    break;
            }
        }

        private void ProcessMaterialRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var materialRequests = View.SelectedObjects.Cast<MaterialRequest>().ToList();
            MessageOptions messageOptions = null;

            var service = new MaterialRequestService();
            var request = new ProcessMaterialRequest()
            {
                UserName = SecuritySystem.CurrentUserName,
                MaterialRequestIds = materialRequests.Select(x => x.Id).ToList()
            };

            var response = service.Process(request).Result;

            if (response != null)
            {
                if(response.Result.Code == "000")
                {
                    messageOptions = Message.GetMessageOptions("Action successfully", "Error", 
                        InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(response.Result.Message, "Error", 
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null,
                    5000);
            }

            if (messageOptions != null)
                Application.ShowViewStrategy.ShowMessage(messageOptions);
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
