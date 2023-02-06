using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class DailyFinishGoodReceiptParamAction_ViewController 
        : ObjectViewController<DetailView, DailyFinishGoodReceiptParam>
    {
        public DailyFinishGoodReceiptParamAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchStyleForFinishGoodReceiptParamAction = new SimpleAction(this, "SearchStyleForFinishGoodReceiptParamAction", PredefinedCategory.Unspecified);
            searchStyleForFinishGoodReceiptParamAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchStyleForFinishGoodReceiptParamAction.Caption = "Search (Ctrl + L)";
            searchStyleForFinishGoodReceiptParamAction.TargetObjectType = typeof(DailyFinishGoodReceiptParam);
            searchStyleForFinishGoodReceiptParamAction.TargetViewType = ViewType.DetailView;
            searchStyleForFinishGoodReceiptParamAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchStyleForFinishGoodReceiptParamAction.Shortcut = "CtrlL";

            searchStyleForFinishGoodReceiptParamAction.Execute += 
                SearchStyleForFinishGoodReceiptParamAction_Execute;

            SimpleAction submitFinishGoodReceiptAction = new SimpleAction(this, "submitFinishGoodReceiptAction", PredefinedCategory.Unspecified);
            submitFinishGoodReceiptAction.ImageName = "Send";
            submitFinishGoodReceiptAction.Caption = "Submit (Shift + S)";
            submitFinishGoodReceiptAction.TargetObjectType = typeof(DailyFinishGoodReceiptParam);
            submitFinishGoodReceiptAction.TargetViewType = ViewType.DetailView;
            submitFinishGoodReceiptAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            submitFinishGoodReceiptAction.Shortcut = "ShiftS";

            submitFinishGoodReceiptAction.Execute += SubmitFinishGoodReceiptAction_Execute;
        }

        private void SubmitFinishGoodReceiptAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as DailyFinishGoodReceiptParam;
            MessageOptions message = null;

            if(viewObject != null)
            {
                if(viewObject.Details.Where(x => x.EntryQuantity > 0).Any())
                {
                    var service = new ReceiptService();

                    var request = new CreateReceiptRequest()
                    {
                        ArrivedDate = viewObject.ReceiptDate,
                        EntriedDate = viewObject.ReceiptDate,
                        ReceiptDate = viewObject.ReceiptDate,
                        ReceiptBy = SecuritySystem.CurrentUserName,
                        EntriedBy = SecuritySystem.CurrentUserName,
                        CustomerID = viewObject.Customer?.ID,
                        StorageCode = viewObject.Storage?.Code,
                        ReceiptTypeId = "RFG",
                        ReceiptGroupLines = viewObject.Details
                        .Where(x => x.EntryQuantity > 0)
                        .Select(x => new ReceiptGroupLineRequest()
                        {
                            GarmentColorCode = x.GarmentColorCode,
                            GarmentColorName = x.GarmentColorName,
                            GarmentSize = x.GarmentSize,
                            CustomerStyle = x.CustomerStyle,
                            LSStyle = x.LSStyle,
                            Season = x.Season,
                            ReceiptQuantity = x.EntryQuantity,
                            CartonQuantity = x.CartonQuantity,
                        }).ToList(),
                    };

                    var respone = service.CreateReceipt(request).Result;


                    if (respone != null)
                    {
                        if (respone.Result.Code == "100")
                        {
                            message = Message.GetMessageOptions("Create successfully", "Success",
                                InformationType.Success, null, 5000);

                            var criteria = CriteriaOperator.Parse("[ReceiptDate] <= ? AND [ReceiptDate] >= ? AND ReceiptTypeId = 'RFG'",
                                DateTime.Now, DateTime.Today);
                            viewObject.Details = new List<DailyFinishGoodReceiptDetail>();
                            viewObject.Receipts = ObjectSpace.GetObjects<Receipt>(criteria).ToList();
                            
                            View.Refresh();
                        }
                        else
                        {
                            message = Message.GetMessageOptions(respone.Result.Message, "Error",
                                InformationType.Error, null, 5000);
                        }
                    }
                    else
                    {
                        message = Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
                    }

                }
                else
                {
                    message = Message.GetMessageOptions("No entry line", "Information", 
                        InformationType.Info,
                        null, 5000);
                }
               
            }

            if(message != null)
            {
                Application.ShowViewStrategy.ShowMessage(message);
            }
        }

        private void SearchStyleForFinishGoodReceiptParamAction_Execute(object sender, 
            SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as DailyFinishGoodReceiptParam;
            
            if(viewObject != null)
            {
                var itemStyles = ObjectSpace
                    .GetObjects<ItemStyle>(CriteriaOperator
                        .Parse("Contains(PurchaseOrderNumber, ?) OR ? AND SalesOrder.CustomerID = ?",
                        viewObject.PurchaseOrderNumber, string.IsNullOrEmpty(viewObject.PurchaseOrderNumber), viewObject.Customer?.ID))
                    .SelectMany(x => x.OrderDetails)
                    .OrderBy(x => x.SizeSortIndex)
                    .Select(x => new DailyFinishGoodReceiptDetail()
                    {
                        PurchaseOrderNumber = x.ItemStyle?.PurchaseOrderNumber,
                        CustomerStyle = x.ItemStyle?.CustomerStyle,
                        LSStyle = x.ItemStyle?.LSStyle,
                        GarmentColorCode = x.ItemStyle?.ColorCode,
                        GarmentColorName = x.ItemStyle?.ColorName,
                        Season = x.ItemStyle?.Season,
                        GarmentSize = x.Size,
                        OrderQuantity = x.Quantity ?? 0,
                    });
                viewObject.Details = itemStyles.ToList();
                View.Refresh();
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
