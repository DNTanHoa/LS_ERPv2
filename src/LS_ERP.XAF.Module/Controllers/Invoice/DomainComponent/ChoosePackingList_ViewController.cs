using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Process;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class ChoosePackingList_ViewController : ViewController
    {
        public ChoosePackingList_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction popupInvoice = new PopupWindowShowAction(this,
                "PopupInvoice", PredefinedCategory.Unspecified);
            popupInvoice.ImageName = "Header";
            popupInvoice.Caption = "Choose Packing list";
            popupInvoice.TargetObjectType = typeof(Invoice);
            popupInvoice.TargetViewType = ViewType.DetailView;
            popupInvoice.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            //popupInvoice.Shortcut = "CtrlShiftO";

            popupInvoice.CustomizePopupWindowParams += PopupInvoice_CustomizePopupWindowParams;
            popupInvoice.Execute += PopupInvoice_Execute;
        }

        private void PopupInvoice_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as Invoice;
            ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
                .FindItem("ChoosePackingListPopupModel") as ListPropertyEditor;
            var choosePackingListPopupModel = listPropertyEditor.ListView?.SelectedObjects.Cast<ChoosePackingListPopupModel>().ToList();
            var packingLists = objectSpace.GetObjects<PackingList>().Where(p => choosePackingListPopupModel.Select(s => s.ID).ToList().Contains(p.ID)).ToList();
            var messageOptions = new MessageOptions();
            string errorMessage = string.Empty;
            var ifgUnit = ObjectSpace.GetObjectByKey<Unit>("DZ");
            var partPrices = new List<PartPrice>();
            var salesOrders = new List<SalesOrder>();
            var tableName = "";
            var filter = "";
            var styles = packingLists.SelectMany(x => x.ItemStyles).ToList();

            if (string.IsNullOrEmpty(tableName))
                tableName = typeof(PartPrice).Name;

            var connectString = Application.ConnectionString ?? string.Empty;
            using (var db = new QueryFactory(
                new SqlConnection(connectString), new SqlServerCompiler()))
            {
                filter = " [CustomerID] = '" + viewObject?.CustomerID + "'"; 
                       // + "' AND"
                       //+ " CAST([EffectiveDate] AS DATE) <= '" + viewObject?.Date.Value.Date + "' AND"
                       //+ " CAST([ExpiryDate] AS DATE) >= '" + viewObject?.Date.Value.Date + "'";

                partPrices = db.Query(tableName)
                        .WhereRaw(filter).Get<PartPrice>().ToList();

                filter = " [ID] IN (" + string.Join(",", styles.Select(x => "'" + x.SalesOrderID + "'")) + ")";

                salesOrders = db.Query("SalesOrders")
                        .WhereRaw(filter).Get<SalesOrder>().ToList();
            }

            if (viewObject.PackingList == null)
            {
                viewObject.PackingList = new List<PackingList>();
            }

            InvoiceProcessor.CreateOrUpdateInvoiceDetail(viewObject, packingLists, SecuritySystem.CurrentUserName, ifgUnit, partPrices, salesOrders, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                messageOptions = Message.GetMessageOptions(errorMessage, "Error", InformationType.Error, null, 5000);
            }

            ObjectSpace.CommitChanges();

            View.Refresh();
        }

        private void PopupInvoice_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as Invoice;
            
            var model = new ChoosePackingListParam();            
            model.ExistPackingList = viewObject?.PackingList?.ToList();
            model.CustomerID = viewObject.CustomerID;
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = view;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
