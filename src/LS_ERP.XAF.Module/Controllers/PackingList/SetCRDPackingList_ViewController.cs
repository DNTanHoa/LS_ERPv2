using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class SetCRDPackingList_ViewController : ObjectViewController<DetailView, PackingListSearchParam>
    {
        public SetCRDPackingList_ViewController()
        {
            InitializeComponent();            

            PopupWindowShowAction loacdPackingList = new PopupWindowShowAction(this,
                "LoadPackingListForSetCRD", PredefinedCategory.Unspecified);
            loacdPackingList.ImageName = "Header";
            loacdPackingList.Caption = "Set Cargo Ready Date";
            loacdPackingList.TargetObjectType = typeof(PackingListSearchParam);
            loacdPackingList.TargetViewType = ViewType.DetailView;
            loacdPackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;           

            loacdPackingList.CustomizePopupWindowParams += LoadPackingList_CustomizePopupWindowParams;
            loacdPackingList.Execute += SetCRDPackingList_Execute;
        }

        private void SetCRDPackingList_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
           
            ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
                .FindItem("PackingLists") as ListPropertyEditor;
            var packingListModels = listPropertyEditor.ListView?.SelectedObjects.Cast<PackingListPopupModel>().ToList();
            var cargoReadyDate = ((DetailView)e.PopupWindowView).FindItem("CargoReadyDate");
            var packingListSetCRDParam = cargoReadyDate.CurrentObject as PackingListSetCRDParam;
            
            MessageOptions options = null;
            if (cargoReadyDate != null)
            {
                var connectString = Application.ConnectionString ?? string.Empty;
                using (var db = new QueryFactory(
                    new SqlConnection(connectString), new SqlServerCompiler()))
                {
                    var tableName = typeof(PackingList).Name;
                    var PPCBookDateUpdate = db.Query(tableName)
                        .WhereIn("ID", packingListModels.Select(s => s.ID).ToList())
                        .Update(new
                        {
                            PPCBookDate = packingListSetCRDParam.CargoReadyDate,
                            LastUpdatedBy = SecuritySystem.CurrentUserName,
                            LastUpdatedAt = DateTime.Now
                        });
                }
                options = Message.GetMessageOptions("Set Cargo Ready Date successfully. ", "Successs", InformationType.Success, null, 5000);
            }
            Application.ShowViewStrategy.ShowMessage(options);
            View.Refresh();
        }

        private void LoadPackingList_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as PackingListSearchParam;
           
            var model = new PackingListSetCRDParam(); 
            if(viewObject !=null)
            {
                model.Customer = viewObject.Customer;
            }  
            var fromDate = DateTime.Now;
            fromDate = fromDate.AddMonths(-3);
            model.PackingFromDate = fromDate;

            var toDate = DateTime.Now;
            toDate = toDate.AddMonths(1);           
            model.PackingToDate = toDate;
            model.CargoReadyDate = DateTime.Now;
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
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
