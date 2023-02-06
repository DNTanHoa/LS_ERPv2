using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class IssuedContractAction_ViewController : ObjectViewController<DetailView, IssuedContractParam>
    {
        public IssuedContractAction_ViewController()
        {
            InitializeComponent();

            SimpleAction loadContractToIssuedAction = new SimpleAction(this, "LoadContractToIssued", PredefinedCategory.Unspecified);
            loadContractToIssuedAction.ImageName = "Action_Search_Object_FindObjectByID";
            loadContractToIssuedAction.Caption = "Search (Ctrl + L)";
            loadContractToIssuedAction.TargetObjectType = typeof(IssuedContractParam);
            loadContractToIssuedAction.TargetViewType = ViewType.DetailView;
            loadContractToIssuedAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadContractToIssuedAction.Shortcut = "CtrlShiftL";

            loadContractToIssuedAction.Execute += LoadContractToIssuedAction_Execute;

            SimpleAction loadContractItemToIssuedAction = new SimpleAction(this, "LoadContractitemToIssued", PredefinedCategory.Unspecified);
            loadContractItemToIssuedAction.ImageName = "Action_Search_Object_FindObjectByID";
            loadContractItemToIssuedAction.Caption = "Load (Ctrl + B)";
            loadContractItemToIssuedAction.TargetObjectType = typeof(IssuedContractParam);
            loadContractItemToIssuedAction.TargetViewType = ViewType.DetailView;
            loadContractItemToIssuedAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadContractItemToIssuedAction.Shortcut = "CtrlL";

            loadContractItemToIssuedAction.Execute += LoadContractItemToIssuedAction_Execute;

            SimpleAction copyContractQuantityToIssued = new SimpleAction(this, "CopyContractQuantityToIssued", PredefinedCategory.Unspecified);
            copyContractQuantityToIssued.ImageName = "";
            copyContractQuantityToIssued.Caption = "Copy Quantity (Ctrl + E)";
            copyContractQuantityToIssued.TargetObjectType = typeof(IssuedContractParam);
            copyContractQuantityToIssued.TargetViewType = ViewType.DetailView;
            copyContractQuantityToIssued.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            copyContractQuantityToIssued.Shortcut = "CtrlE";

            copyContractQuantityToIssued.Execute += CopyContractQuantityToIssued_Execute;


            PopupWindowShowAction issuedStorageDetail = new PopupWindowShowAction(this, "IssuedStorageDetailAction",
                PredefinedCategory.Unspecified);

            issuedStorageDetail.ImageName = "Header";
            issuedStorageDetail.Caption = "StorageDetail";
            issuedStorageDetail.TargetObjectType = typeof(IssuedContractParam);
            issuedStorageDetail.TargetViewType = ViewType.DetailView;
            issuedStorageDetail.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //issuedStorageDetail.Shortcut = "CtrlShiftD";

            issuedStorageDetail.CustomizePopupWindowParams += IssuedStorageDetailAction_CustomizePopupWindowParams;
            issuedStorageDetail.Execute += IssuedStorageDetailAction_Execute;
        }

        private void CopyContractQuantityToIssued_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as IssuedContractParam;
            ListPropertyEditor listPropertyEditor =
                ((DetailView)View).FindItem("Details") as ListPropertyEditor;

            if (listPropertyEditor != null)
            {
                var details = listPropertyEditor.ListView
                   .SelectedObjects.Cast<IssuedContractDetail>();

                foreach (var detail in details)
                {
                    detail.IssuedQuantity = detail.RequiredQuantity;
                }

                View.Refresh();
            }
        }

        private void LoadContractItemToIssuedAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as IssuedContractParam;
            ListPropertyEditor listPropertyEditor =
                ((DetailView)View).FindItem("Contracts") as ListPropertyEditor;

            if (listPropertyEditor != null)
            {
                var contracts = listPropertyEditor.ListView
                                 .SelectedObjects.Cast<IssuedContractStyle>();

                switch (viewObject.Customer?.ID)
                {
                    case "PU":
                        {
                            var contractNos = "[ContractNo] IN (" +
                                string.Join(",", contracts.Select(x => "'" + x.ContractNo + "'")) + ") AND MaterialType = ?";

                            var criteria = CriteriaOperator.Parse(contractNos, viewObject.MaterialTypeCode);

                            var poLines = ObjectSpace.GetObjects<PurchaseOrderLine>(criteria);
                            var data = poLines.GroupBy(x =>
                                new
                                {
                                    x.ItemID,
                                    x.ItemName,
                                    x.ItemColorCode,
                                    x.ItemColorName,
                                    x.CustomerStyle,
                                    x.UnitID
                                })
                                .Select(x => new IssuedContractDetail()
                                {
                                    ItemID = x.Key.ItemID,
                                    ItemName = x.Key.ItemName,
                                    ItemColorCode = x.Key.ItemColorCode,
                                    ItemColorName = x.Key.ItemColorName?.Replace("\n", ""),
                                    CustomStyle = x.Key.CustomerStyle,
                                    UnitID = x.Key.UnitID,
                                    RequiredQuantity = x.Sum(x => x.Quantity) ?? 0

                                });

                            viewObject.Details = data.ToList();
                        }
                        break;
                    default:
                        {
                            var contractNos = "[ItemStyle.ContractNo] IN (" +
                                string.Join(",", contracts.Select(x => "'" + x.ContractNo + "'")) + ") AND MaterialTypeCode = ?";

                            var criteria = CriteriaOperator.Parse(contractNos, viewObject.MaterialTypeCode);

                            var proBoms = ObjectSpace.GetObjects<ProductionBOM>(criteria);
                            var data = proBoms.GroupBy(x =>
                                new { x.ItemID, x.ItemName, x.ItemColorCode, x.ItemColorName, x.ItemStyle?.CustomerStyle, x.PerUnitID })
                                .Select(x => new IssuedContractDetail()
                                {
                                    ItemID = x.Key.ItemID,
                                    ItemName = x.Key.ItemName,
                                    ItemColorCode = x.Key.ItemColorCode,
                                    ItemColorName = x.Key.ItemColorName?.Replace("\n", ""),
                                    CustomStyle = x.Key.CustomerStyle,
                                    UnitID = x.Key.PerUnitID,
                                    RequiredQuantity = x.Sum(x => x.ConsumptionQuantity) ?? 0
                                });

                            viewObject.Details = data.ToList();
                        }
                        break;
                }
                View.Refresh();
            }
        }

        private void LoadContractToIssuedAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as IssuedContractParam;

            if (viewObject != null)
            {
                var criteria = CriteriaOperator.Parse("[SalesOrder.Customer] = ? AND " +
                    "(Contains(ContractNo,?) OR ?)",
                        viewObject.Customer, viewObject.ContractSearch, string.IsNullOrEmpty(viewObject.ContractSearch));
                var itemStyles = ObjectSpace.GetObjects<ItemStyle>(criteria);
                var data = itemStyles
                .GroupBy(x => new { x.ContractNo, x.ColorCode, x.ColorName, x.Season })
                .Select(x => new IssuedContractStyle()
                {
                    ContractNo = x.Key.ContractNo,
                    GarmentColorCode = x.Key.ColorCode,
                    GarmentColorName = x.Key.ColorName,
                    Season = x.Key.Season
                });

                viewObject.Contracts = data.ToList();
                View.Refresh();
            }
        }

        private void IssuedStorageDetailAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = objectSpace.CreateObject<FabricStorageParam>();


            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = view;
        }

        private void IssuedStorageDetailAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            //var viewObject = e.PopupWindowViewCurrentObject as IssuedContractParam;

            //MessageOptions message = null;
            //var service = new IssuedService();

            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<IssuedContractParam, CreateIssuedRequest>()
            //        .ForMember(x => x.CustomerID, y => y.MapFrom(s => s.Customer.ID))
            //        .ForMember(x => x.IssuedGroupLines, y => y.MapFrom(s => s.Details.Where(d => d.IssuedQuantity > 0)));
            //    cfg.CreateMap<Issued, UpdateIssuedRequest>();
            //    cfg.CreateMap<IssuedContractDetail, IssuedGroupLineDto>();
            //    cfg.CreateMap<IssuedLine, IssuedLineDto>();
            //});

            //var mapper = config.CreateMapper();

            //var request = mapper.Map<CreateIssuedRequest>(viewObject);

            //request.IssuedGroupLines.ForEach(x =>
            //{
            //    x.IssuedLines = new List<IssuedLineDto>()
            //    {
            //        new IssuedLineDto()
            //        {
            //            ItemID = x.ItemID,
            //            DsmItemID = x.DsmItemID,
            //            ItemCode = x.ItemCode,
            //            ItemName = x.ItemName.Replace("\n",""),
            //            ItemColorCode = x.ItemColorCode.Replace("\n",""),
            //            ItemColorName = x.ItemColorName.Replace("\n",""),
            //            Position = x.Position,
            //            Specify = x.Specify,
            //            Season = x.Season,
            //            UnitID = x.UnitID,
            //            CustomerStyle = x.CustomerStyle,
            //            GarmentColorCode = x.GarmentColorCode,
            //            GarmentColorName = x.GarmentColorName,
            //            IssuedQuantity = x.IssuedQuantity,
            //            Roll = x.Roll
            //        }
            //    };
            //});

            //var response = service.CreateIssued(request).Result;

            //if (response != null)
            //{
            //    if (response.Result.Code == "000")
            //    {
            //        message = Message.GetMessageOptions("Action successfully", "Success", InformationType.Success,
            //            null, 5000);
            //    }
            //    else
            //    {
            //        message = Message.GetMessageOptions(response.Result.Message, "Error", InformationType.Error,
            //            null, 5000);
            //    }
            //}
            //else
            //{
            //    message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error", InformationType.Error,
            //            null, 5000);
            //}

            //if (message != null)
            //    Application.ShowViewStrategy.ShowMessage(message);
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
