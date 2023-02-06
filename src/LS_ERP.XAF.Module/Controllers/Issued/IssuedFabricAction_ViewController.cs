using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Response;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class IssuedFabricAction_ViewController
        : ObjectViewController<DetailView, IssuedFabricParam>
    {
        public IssuedFabricAction_ViewController()
        {
            InitializeComponent();

            //PopupWindowShowAction issuedFabricActionCMT = new PopupWindowShowAction(this, "FabricStorageCMTAction",
            //    PredefinedCategory.Unspecified);

            //issuedFabricActionCMT.ImageName = "Header";
            //issuedFabricActionCMT.Caption = "Fabric Storage (CMT)";
            //issuedFabricActionCMT.TargetObjectType = typeof(IssuedFabricParam);
            //issuedFabricActionCMT.TargetViewType = ViewType.Any;
            //issuedFabricActionCMT.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //issuedFabricActionCMT.Shortcut = "CtrlShiftF";

            //issuedFabricActionCMT.CustomizePopupWindowParams += IssuedFabricCMT_PopupAction_CustomizePopupWindowParams;
            //issuedFabricActionCMT.Execute += IssuedFabricCMT_PopupAction_Execute;


            PopupWindowShowAction issuedFabricAction = new PopupWindowShowAction(this, "FabricStorageAction",
                PredefinedCategory.Unspecified);

            issuedFabricAction.ImageName = "Header";
            issuedFabricAction.Caption = "Fabric Storage";
            issuedFabricAction.TargetObjectType = typeof(IssuedFabricParam);
            issuedFabricAction.TargetViewType = ViewType.Any;
            issuedFabricAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            issuedFabricAction.Shortcut = "CtrlShiftF";

            issuedFabricAction.CustomizePopupWindowParams += IssuedFabricPopupAction_CustomizePopupWindowParams;
            issuedFabricAction.Execute += IssuedFabricPopupAction_Execute;
        }

        private void IssuedFabricPopupAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as IssuedFabricParam;
            var Object = Application.CreateObjectSpace(typeof(Issued));

            if (viewObject != null && viewObject.Customer != null)
            {
                if (viewObject.FabricRequestDetails != null && viewObject.FabricRequestDetails.Count > 0)
                {
                    var model = objectSpace.CreateObject<FabricStorageParam>();
                    var service = new StorageDetailService();
                    var response = service.GetReport(viewObject.Customer?.ID ?? string.Empty, viewObject.Storage?.Code,
                        null, null, viewObject.ProductionMethodCode?.Code, 0).Result;

                    if (response.IsSuccess)
                    {
                        var mapperConfig = new MapperConfiguration(c =>
                        {
                            c.CreateMap<StorageDetailReportResponseData, StorageDetailsReport>();
                        });

                        var mapper = mapperConfig.CreateMapper();

                        var selectFBStorage = response.Data
                            .Select(x => mapper.Map<StorageDetailsReport>(x)).Where(x => x.OnHandQuantity > 0).ToList();



                        //var criteriaString = "StorageCode = 'FB' AND OnHandQuantity > 0 AND CustomerID = '" + viewObject.Customer.ID + "'" +
                        //    " AND ProductionMethodCode <> 'CMT' ";

                        //var selectFBStorage = Object.GetObjects<StorageDetail>(CriteriaOperator.Parse(criteriaString)).ToList();

                        ListPropertyEditor listPropertyEditorCurrentObject = ((DetailView)View)
                        .FindItem("FabricRequestDetails") as ListPropertyEditor;

                        var selectedRequest = listPropertyEditorCurrentObject?.ListView?.SelectedObjects?.Cast<FabricRequestDetail>()?.ToList();

                        if (selectedRequest != null && selectedRequest.Count == 1)
                        {
                            if (viewObject.Fabrics != null && viewObject.Fabrics.Count > 0)
                            {
                                var selectedItem = selectedRequest.FirstOrDefault();
                                var idDetail = selectedItem.ID;
                                var totalQuantity = selectedItem.RequestQuantity + selectedItem.StreakRequestQuantity;

                                var deleteFBStorage = new List<StorageDetailsReport>();

                                foreach (var itemStorage in selectFBStorage)
                                {
                                    foreach (var itemFabric in viewObject.Fabrics)
                                    {
                                        if (itemFabric.StorageDetailID == itemStorage.ID && itemFabric.FabricRequestDetailID == idDetail)
                                        {
                                            itemStorage.OnHandQuantity -= itemFabric.IssuedQuantity;
                                            totalQuantity -= itemFabric.IssuedQuantity;
                                            if (itemStorage.OnHandQuantity <= 0)
                                            {
                                                deleteFBStorage.Add(itemStorage);
                                            }
                                        }
                                        else if (itemFabric.StorageDetailID == itemStorage.ID)
                                        {
                                            itemStorage.OnHandQuantity -= itemFabric.IssuedQuantity;
                                            if (itemStorage.OnHandQuantity <= 0)
                                            {
                                                deleteFBStorage.Add(itemStorage);
                                            }
                                        }
                                    }
                                }

                                foreach (var item in selectFBStorage)
                                {
                                    if (item.OnHandQuantity <= 0)
                                    {
                                        deleteFBStorage.Add(item);
                                    }
                                }

                                foreach (var itemDelete in deleteFBStorage)
                                {
                                    selectFBStorage.Remove(itemDelete);
                                }

                                if (totalQuantity <= 0)
                                {
                                    model.Storage = new List<StorageDetailsReport>();
                                }
                                else
                                {
                                    model.Storage = selectFBStorage;
                                }
                            }
                            else
                            {
                                model.Storage = selectFBStorage;
                            }
                        }
                        else
                        {
                            var infor = Message.GetMessageOptions("Please choose only one Fabric request item", "infor",
                        InformationType.Info, null, 2000);
                            Application.ShowViewStrategy.ShowMessage(infor);
                        }

                        //viewObject.ProductionMethodCode = "";

                        var view = Application.CreateDetailView(objectSpace, model, false);
                        e.DialogController.SaveOnAccept = false;
                        e.Maximized = true;
                        e.View = view;

                        //View.Refresh();
                    }
                    else
                    {
                        var message = Message.GetMessageOptions(response.ErrorMessage, "Error",
                            InformationType.Error, null, 5000);
                        Application.ShowViewStrategy.ShowMessage(message);
                    }
                }
                else
                {
                    var model = objectSpace.CreateObject<FabricStorageParam>();

                    //var criteriaString = "StorageCode = 'FB' AND OnHandQuantity > 0 AND CustomerID = '" + viewObject.Customer.ID + "'" +
                    //    " AND ProductionMethodCode <> 'CMT' ";

                    //model.Storage = Object.GetObjects<StorageDetail>(CriteriaOperator.Parse(criteriaString)).ToList();

                    var service = new StorageDetailService();
                    var response = service.GetReport(viewObject.Customer?.ID ?? string.Empty, viewObject.Storage?.Code,
                        null, null, viewObject.ProductionMethodCode?.Code, 0).Result;

                    if (response.IsSuccess)
                    {
                        var mapperConfig = new MapperConfiguration(c =>
                        {
                            c.CreateMap<StorageDetailReportResponseData, StorageDetailsReport>();
                        });

                        var mapper = mapperConfig.CreateMapper();

                        model.Storage = response.Data
                            .Select(x => mapper.Map<StorageDetailsReport>(x)).Where(x => x.OnHandQuantity > 0).ToList();

                        //viewObject.ProductionMethodCode = "";

                        var view = Application.CreateDetailView(objectSpace, model, false);
                        e.DialogController.SaveOnAccept = false;
                        e.Maximized = true;
                        e.View = view;
                    }
                    else
                    {
                        var message = Message.GetMessageOptions(response.ErrorMessage, "Error",
                            InformationType.Error, null, 5000);
                        Application.ShowViewStrategy.ShowMessage(message);
                    }


                }
            }
            else
            {
                var infor = Message.GetMessageOptions("Please choose Customer", "infor",
                    InformationType.Info, null, 2000);
                Application.ShowViewStrategy.ShowMessage(infor);
            }

        }

        private void IssuedFabricPopupAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;

            var viewObject = View.CurrentObject as IssuedFabricParam;

            ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
                .FindItem("Storage") as ListPropertyEditor;

            ListPropertyEditor listPropertyEditorCurrentObject = ((DetailView)View)
                .FindItem("FabricRequestDetails") as ListPropertyEditor;

            var selectedRequest = listPropertyEditorCurrentObject?.ListView?.SelectedObjects?.Cast<FabricRequestDetail>().ToList().FirstOrDefault();

            long fabricRequestDetailID = 0;

            if (listPropertyEditor != null)
            {
                var selecteds = listPropertyEditor.ListView.SelectedObjects.Cast<StorageDetailsReport>().ToList();

                if (selectedRequest != null)
                {
                    fabricRequestDetailID = selectedRequest.ID;
                    var requestQty = selectedRequest.RequestQuantity ?? selectedRequest.StreakRequestQuantity;

                    var selectFabric = selecteds
                    .Select(x => new IssuedFabric()
                    {
                        CustomerStyle = x.CustomerStyle,
                        InStockQuantity = x.OnHandQuantity ?? 0,
                        FabricPurchaseOrderNumber = x.FabricPurchaseOrderNumber,
                        PurchaseOrderNumber = x.PurchaseOrderNumber,
                        StorageDetailID = x.ID,
                        ItemID = x.ItemID,
                        UnitID = x.UnitID,
                        ItemName = x.ItemName,
                        ItemColorCode = x.ItemColorCode,
                        ItemColorName = x.ItemColorName,
                        Season = x.Season,
                        StorageStatusID = x.StorageStatusID,
                        LotNumber = x.LotNumber,
                        RollInStock = x.Roll,
                        DyeLotNumber = x.DyeLotNumber,
                        InvoiceNumber = x.InvoiceNumber,
                        InvoiceNumberNoTotal = x.InvoiceNumberNoTotal

                    }).ToList();


                    foreach (var item in selectFabric)
                    {
                        item.FabricRequestDetailID = fabricRequestDetailID;
                        item.RequestQuantity = requestQty;

                        viewObject.Fabrics.Add(item);
                    }
                }
                else
                {
                    viewObject.Fabrics = selecteds
                                        .Select(x => new IssuedFabric()
                                        {
                                            CustomerStyle = x.CustomerStyle,
                                            InStockQuantity = x.OnHandQuantity ?? 0,
                                            FabricPurchaseOrderNumber = x.FabricPurchaseOrderNumber,
                                            PurchaseOrderNumber = x.PurchaseOrderNumber,
                                            StorageDetailID = x.ID,
                                            ItemID = x.ItemID,
                                            UnitID = x.UnitID,
                                            ItemName = x.ItemName,
                                            ItemColorCode = x.ItemColorCode,
                                            ItemColorName = x.ItemColorName,
                                            Season = x.Season,
                                            StorageStatusID = x.StorageStatusID,
                                            LotNumber = x.LotNumber,
                                            RollInStock = x.Roll,
                                            DyeLotNumber = x.DyeLotNumber,
                                            InvoiceNumber = x.InvoiceNumber,
                                            InvoiceNumberNoTotal = x.InvoiceNumberNoTotal

                                        }).ToList();
                }
                View.Refresh();
            }
        }

        private void IssuedFabricCMT_PopupAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as IssuedFabricParam;
            var Object = Application.CreateObjectSpace(typeof(Issued));

            if (viewObject != null && viewObject.Customer != null)
            {
                //viewObject.ProductionMethodCode = "CMT";

                if (viewObject.FabricRequestDetails != null && viewObject.FabricRequestDetails.Count > 0)
                {
                    var model = objectSpace.CreateObject<FabricStorageParam>();

                    var service = new StorageDetailService();
                    var response = service.GetReport(viewObject.Customer?.ID ?? string.Empty, viewObject.Storage?.Code,
                        null, null, viewObject.ProductionMethodCode?.Code, 0).Result;

                    if (response.IsSuccess)
                    {
                        var mapperConfig = new MapperConfiguration(c =>
                        {
                            c.CreateMap<StorageDetailReportResponseData, StorageDetailsReport>();
                        });

                        var mapper = mapperConfig.CreateMapper();

                        var selectFBStorage = response.Data
                            .Select(x => mapper.Map<StorageDetailsReport>(x)).Where(x => x.OnHandQuantity > 0).ToList();
                        //&& (x.FabricPurchaseOrderNumber != null && x.FabricPurchaseOrderNumber != "")).ToList();

                        ListPropertyEditor listPropertyEditorCurrentObject = ((DetailView)View)
                    .FindItem("FabricRequestDetails") as ListPropertyEditor;

                        var selectedRequest = listPropertyEditorCurrentObject?.ListView?.SelectedObjects?.Cast<FabricRequestDetail>()?.ToList();

                        if (selectedRequest != null && selectedRequest.Count == 1)
                        {
                            if (viewObject.Fabrics != null && viewObject.Fabrics.Count > 0)
                            {
                                var selectedItem = selectedRequest.FirstOrDefault();
                                var idDetail = selectedItem.ID;
                                var totalQuantity = selectedItem.RequestQuantity + selectedItem.StreakRequestQuantity;

                                var deleteFBStorage = new List<StorageDetailsReport>();

                                foreach (var itemStorage in selectFBStorage)
                                {
                                    foreach (var itemFabric in viewObject.Fabrics)
                                    {
                                        if (itemFabric.StorageDetailID == itemStorage.ID && itemFabric.FabricRequestDetailID == idDetail)
                                        {
                                            itemStorage.OnHandQuantity -= itemFabric.IssuedQuantity;
                                            totalQuantity -= itemFabric.IssuedQuantity;
                                            if (itemStorage.OnHandQuantity <= 0)
                                            {
                                                deleteFBStorage.Add(itemStorage);
                                            }
                                        }
                                        else if (itemFabric.StorageDetailID == itemStorage.ID)
                                        {
                                            itemStorage.OnHandQuantity -= itemFabric.IssuedQuantity;
                                            if (itemStorage.OnHandQuantity <= 0)
                                            {
                                                deleteFBStorage.Add(itemStorage);
                                            }
                                        }
                                    }
                                }

                                foreach (var item in selectFBStorage)
                                {
                                    if (item.OnHandQuantity <= 0)
                                    {
                                        deleteFBStorage.Add(item);
                                    }
                                }

                                foreach (var itemDelete in deleteFBStorage)
                                {
                                    selectFBStorage.Remove(itemDelete);
                                }

                                if (totalQuantity <= 0)
                                {
                                    model.Storage = new List<StorageDetailsReport>();
                                }
                                else
                                {
                                    model.Storage = selectFBStorage;
                                }
                            }
                            else
                            {
                                model.Storage = selectFBStorage;
                            }
                        }
                        else
                        {
                            var infor = Message.GetMessageOptions("Please choose only one Fabric request item", "infor",
                        InformationType.Info, null, 2000);
                            Application.ShowViewStrategy.ShowMessage(infor);
                        }


                        var view = Application.CreateDetailView(objectSpace, model, false);

                        e.DialogController.SaveOnAccept = false;
                        e.Maximized = true;
                        e.View = view;

                    }
                    else
                    {
                        var message = Message.GetMessageOptions(response.ErrorMessage, "Error",
                            InformationType.Error, null, 5000);
                        Application.ShowViewStrategy.ShowMessage(message);
                    }

                    //var criteriaString = "StorageCode = 'FB' AND OnHandQuantity > 0 AND CustomerID = '" + viewObject.Customer.ID + "'" +
                    //    " AND ProductionMethodCode = 'CMT' AND (FabricPurchaseOrderNumber is not null AND FabricPurchaseOrderNumber <> '' ) ";

                    //var selectFBStorage = Object.GetObjects<StorageDetail>(CriteriaOperator.Parse(criteriaString)).ToList();
                    //viewObject.ProductionMethodCode = "CMT";


                }
                else
                {
                    var model = objectSpace.CreateObject<FabricStorageParam>();

                    //var criteriaString = "StorageCode = 'FB' AND OnHandQuantity > 0 AND CustomerID = '" + viewObject.Customer.ID + "'" +
                    //    " AND ProductionMethodCode = 'CMT' AND (FabricPurchaseOrderNumber is not null AND FabricPurchaseOrderNumber <> '' ) ";

                    //model.Storage = Object.GetObjects<StorageDetail>(CriteriaOperator.Parse(criteriaString)).ToList();

                    var service = new StorageDetailService();
                    var response = service.GetReport(viewObject.Customer?.ID ?? string.Empty, viewObject.Storage?.Code,
                        null, null, viewObject.ProductionMethodCode?.Code, 0).Result;

                    if (response.IsSuccess)
                    {
                        var mapperConfig = new MapperConfiguration(c =>
                        {
                            c.CreateMap<StorageDetailReportResponseData, StorageDetailsReport>();
                        });

                        var mapper = mapperConfig.CreateMapper();

                        model.Storage = response.Data
                            .Select(x => mapper.Map<StorageDetailsReport>(x)).Where(x => x.OnHandQuantity > 0).ToList();
                        //&& (x.FabricPurchaseOrderNumber != null && x.FabricPurchaseOrderNumber != "")).ToList();

                        var view = Application.CreateDetailView(objectSpace, model, false);
                        e.DialogController.SaveOnAccept = false;
                        e.Maximized = true;
                        e.View = view;
                    }
                    else
                    {
                        var message = Message.GetMessageOptions(response.ErrorMessage, "Error",
                            InformationType.Error, null, 5000);
                        Application.ShowViewStrategy.ShowMessage(message);
                    }


                }
            }
            else
            {
                var infor = Message.GetMessageOptions("Please choose Customer", "infor",
                    InformationType.Info, null, 2000);
                Application.ShowViewStrategy.ShowMessage(infor);
            }

        }

        private void IssuedFabricCMT_PopupAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;

            var viewObject = View.CurrentObject as IssuedFabricParam;
            ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
               .FindItem("Storage") as ListPropertyEditor;

            ListPropertyEditor listPropertyEditorCurrentObject = ((DetailView)View)
                .FindItem("FabricRequestDetails") as ListPropertyEditor;

            var selectedRequest = listPropertyEditorCurrentObject?.ListView?.SelectedObjects?.Cast<FabricRequestDetail>().ToList().FirstOrDefault();

            long fabricRequestDetailID = 0;

            if (listPropertyEditor != null)
            {
                var selecteds = listPropertyEditor.ListView.SelectedObjects.Cast<StorageDetailsReport>().ToList();

                if (selectedRequest != null)
                {
                    fabricRequestDetailID = selectedRequest.ID;
                    var requestQty = selectedRequest.RequestQuantity ?? selectedRequest.StreakRequestQuantity;

                    var selectFabric = selecteds
                    .Select(x => new IssuedFabric()
                    {
                        CustomerStyle = x.CustomerStyle,
                        InStockQuantity = x.OnHandQuantity ?? 0,
                        FabricPurchaseOrderNumber = x.FabricPurchaseOrderNumber,
                        PurchaseOrderNumber = x.PurchaseOrderNumber,
                        StorageDetailID = x.ID,
                        ItemID = x.ItemID,
                        UnitID = x.UnitID,
                        ItemName = x.ItemName,
                        ItemColorCode = x.ItemColorCode,
                        ItemColorName = x.ItemColorName,
                        Season = x.Season,
                        StorageStatusID = x.StorageStatusID,
                        LotNumber = x.LotNumber,
                        RollInStock = x.Roll,
                        DyeLotNumber = x.DyeLotNumber,
                        InvoiceNumber = x.InvoiceNumber,
                        InvoiceNumberNoTotal = x.InvoiceNumberNoTotal

                    }).ToList();


                    foreach (var item in selectFabric)
                    {
                        item.FabricRequestDetailID = fabricRequestDetailID;
                        item.RequestQuantity = requestQty;
                        viewObject.Fabrics.Add(item);
                    }
                }
                else
                {
                    viewObject.Fabrics = selecteds
                                        .Select(x => new IssuedFabric()
                                        {
                                            CustomerStyle = x.CustomerStyle,
                                            InStockQuantity = x.OnHandQuantity ?? 0,
                                            FabricPurchaseOrderNumber = x.FabricPurchaseOrderNumber,
                                            PurchaseOrderNumber = x.PurchaseOrderNumber,
                                            StorageDetailID = x.ID,
                                            ItemID = x.ItemID,
                                            UnitID = x.UnitID,
                                            ItemName = x.ItemName,
                                            ItemColorCode = x.ItemColorCode,
                                            ItemColorName = x.ItemColorName,
                                            Season = x.Season,
                                            StorageStatusID = x.StorageStatusID,
                                            LotNumber = x.LotNumber,
                                            RollInStock = x.Roll,
                                            DyeLotNumber = x.DyeLotNumber,
                                            InvoiceNumber = x.InvoiceNumber,
                                            InvoiceNumberNoTotal = x.InvoiceNumberNoTotal

                                        }).ToList();
                }

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
