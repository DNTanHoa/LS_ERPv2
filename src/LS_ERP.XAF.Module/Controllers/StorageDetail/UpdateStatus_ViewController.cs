using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Response;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Ultils.Extensions;
using Ultils.Helpers;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class UpdateStatus_ViewController : ObjectViewController<DetailView, StorageDetailReportParam>
    {
        public UpdateStatus_ViewController()
        {
            InitializeComponent();            

            PopupWindowShowAction loacdPackingList = new PopupWindowShowAction(this,
                "UpdateStatus", PredefinedCategory.Unspecified);
            loacdPackingList.ImageName = "Update";
            loacdPackingList.Caption = "Update Status (Ctrl + S)";
            loacdPackingList.TargetObjectType = typeof(StorageDetailReportParam);
            loacdPackingList.TargetViewType = ViewType.DetailView;
            loacdPackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loacdPackingList.Shortcut = "CtrlS";


            loacdPackingList.CustomizePopupWindowParams += UpdateStatus_CustomizePopupWindowParams;
            loacdPackingList.Execute += UpdateStatus_Execute;
        }

        public void UpdateStatus_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var storageBinCode = ((DetailView)e.PopupWindowView).FindItem("Status");
            var updateStatusParam = storageBinCode.CurrentObject as UpdateStatusParam;
            var updateStatusStorageDetailReports = updateStatusParam.Details;
            var status = updateStatusParam.Status;
            var reason= updateStatusParam.Reason;
            MessageOptions options = null;
            if (status !=null && updateStatusStorageDetailReports.Any())
            {
                var connectString = Application.ConnectionString ?? string.Empty;
                using (var db = new QueryFactory(
                    new SqlConnection(connectString), new SqlServerCompiler()))
                {
                    var tableName = "StorageDetail";
                    var StorageDetailUpdateStatus = db.Query(tableName)
                        .WhereIn("ID", updateStatusStorageDetailReports.Select(s => s.ID).ToList())
                        .Update(new
                        {
                            StorageStatusID = status.ID,
                            Note = reason
                        });
                }
                options = Message.GetMessageOptions("Update Status successfully. ", "Successs", InformationType.Success, null, 5000);
            }
            Application.ShowViewStrategy.ShowMessage(options);
            //
            foreach(var item in updateStatusStorageDetailReports)
            {
                item.StorageStatusID = status.ID;
            }    
            
            View.Refresh();
        }

        private void UpdateStatus_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as StorageDetailReportParam;

            ListPropertyEditor listPropertyEditor = ((DetailView)View)
              .FindItem("Details") as ListPropertyEditor;

            var updateStatusStorageDetailReports = listPropertyEditor.ListView
                .SelectedObjects.Cast<StorageDetailsReport>().ToList();

            var model = new UpdateStatusParam();
            if(viewObject != null)
            {               
                model.Details = updateStatusStorageDetailReports;
                var view = Application.CreateDetailView(objectSpace, model, false);
                e.DialogController.SaveOnAccept = false;
                e.Maximized = true;
                e.View = view;                
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
