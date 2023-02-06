using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class StorageBinEntryInputAction_ViewController : ViewController
    {
        public StorageBinEntryInputAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchStorageBinEntry = new SimpleAction(this, "SearchStorageBinEntry", PredefinedCategory.Unspecified);
            searchStorageBinEntry.ImageName = "Action_Search_Object_FindObjectByID";
            searchStorageBinEntry.Caption = "Search (Ctrl + L)";
            searchStorageBinEntry.TargetObjectType = typeof(StorageBinEntrySearchParam);
            searchStorageBinEntry.TargetViewType = ViewType.DetailView;
            searchStorageBinEntry.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchStorageBinEntry.Execute += SearchStorageBinEntry_Execute;

            PopupWindowShowAction inputStorageBinEntry = new PopupWindowShowAction(this, "InputStorageBinEntry", PredefinedCategory.Unspecified);
            inputStorageBinEntry.ImageName = "Action_Document_Object_Inplace";
            inputStorageBinEntry.Caption = "Entry (Ctrl + E)";
            inputStorageBinEntry.TargetObjectType = typeof(StorageBinEntrySearchParam);
            inputStorageBinEntry.TargetViewType = ViewType.DetailView;
            inputStorageBinEntry.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            inputStorageBinEntry.CustomizePopupWindowParams += InputStorageBinEntry_CustomizePopupWindowParams;
            inputStorageBinEntry.Execute += InputStorageBinEntry_Execute;
        }

        private void InputStorageBinEntry_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as StorageBinEntrySearchParam;
            var model = new StorageBinEntryCreateParam()
            {
                Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(),
                Factory = objectSpace.GetObjects<Company>().FirstOrDefault(),
                Storage = objectSpace.GetObjects<Storage>().FirstOrDefault(),
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.Maximized = true;
            e.DialogController.SaveOnAccept = false;
            e.View = view;
        }
        private void InputStorageBinEntry_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowViewCurrentObject as StorageBinEntryCreateParam;
            var service = new StorageBinEntryService();
            var messageOptions = new MessageOptions();

            var request = new BulkStorageBinEntryRequest()
            {
                CustomerID = viewObject.Customer?.ID,
                StorageCode = viewObject.Storage?.Code,
                Factory = viewObject.Factory?.Code,
                UserName = SecuritySystem.CurrentUserName,
                Data = viewObject.Data
            };
            var bulkResponse = service.Bulk(request).Result;

            if (bulkResponse != null)
            {
                if (bulkResponse.Success)
                {
                    messageOptions = Message.GetMessageOptions("Action successfully", "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(bulkResponse.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                messageOptions = Message.GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
            }

            View.Refresh();

            Application.ShowViewStrategy.ShowMessage(messageOptions);
        }

        #region OldCode
        //private void InputStorageBinEntry_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        //{
        //    var objectSpace = this.ObjectSpace;
        //    var model = new FinishGoodBinEntryParam()
        //    {
        //        Storage = objectSpace.GetObjects<Storage>().FirstOrDefault(),
        //        Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(),
        //    };
        //    var view = Application.CreateDetailView(objectSpace, model, false);
        //    e.Maximized = true;
        //    e.DialogController.SaveOnAccept = false;
        //    e.View = view;
        //}
        //private void InputStorageBinEntry_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        //{
        //    var viewObject = e.PopupWindowViewCurrentObject as FinishGoodBinEntryParam;
        //    MessageOptions options = null;

        //    if (viewObject != null)
        //    {
        //        var entries = viewObject.Entries.Where(x => !string.IsNullOrEmpty(x.BinCode));
        //        var objectSpace = Application.CreateObjectSpace(typeof(StorageBinEntry));

        //        foreach (var entry in entries)
        //        {
        //            var persistEntry = objectSpace.CreateObject<StorageBinEntry>();
        //            persistEntry.CustomerID = viewObject.Customer?.ID;
        //            persistEntry.StorageCode = viewObject.Storage?.Code;
        //            persistEntry.EntryDate = DateTime.Now;
        //            persistEntry.PurchaseOrderNumber = entry.PurchaseOrderNumber;
        //            persistEntry.CustomerStyle = entry.CustomerStyle;
        //            persistEntry.LSStyle = entry.LSStyle;
        //            persistEntry.GarmentColorCode = entry.GarmentColorCode;
        //            persistEntry.GarmentColorName = entry.GarmentColorName;
        //            persistEntry.Season = entry.Season;
        //            persistEntry.BinCode = entry.BinCode;
        //        }

        //        objectSpace.CommitChanges();
        //        View.Refresh();

        //        options = Message.GetMessageOptions("Action successfully", "Success", InformationType.Success, null, 5000);
        //        Application.ShowViewStrategy.ShowMessage(options);
        //    }
        //}
        #endregion
        private void SearchStorageBinEntry_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as StorageBinEntrySearchParam;

            if (viewObject != null)
            {
                var criteria = CriteriaOperator.Parse("(Contains(LSStyle, ?) OR ?) " +
                    "AND (Contains(PurchaseOrderNumber, ?) OR ?) " +
                    "AND EntryDate >= ? " +
                    "AND EntryDate <= ? ",
                    viewObject.Style, string.IsNullOrEmpty(viewObject.Style),
                    viewObject.PurchaseOrderNumber, string.IsNullOrEmpty(viewObject.PurchaseOrderNumber),
                    viewObject.FromDate, viewObject.ToDate);
                var entries = ObjectSpace.GetObjects<StorageBinEntry>(criteria);

                viewObject.Entries = entries.ToList();
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
