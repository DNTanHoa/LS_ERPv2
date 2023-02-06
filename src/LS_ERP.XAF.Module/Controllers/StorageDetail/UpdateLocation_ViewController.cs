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
    public partial class UpdateLocation_ViewController : ObjectViewController<DetailView, StorageDetailReportParam>
    {
        public UpdateLocation_ViewController()
        {
            InitializeComponent();            

            PopupWindowShowAction loacdPackingList = new PopupWindowShowAction(this,
                "UpdateLocation", PredefinedCategory.Unspecified);
            loacdPackingList.ImageName = "Update";
            loacdPackingList.Caption = "Update Location (Ctrl + U)";
            loacdPackingList.TargetObjectType = typeof(StorageDetailReportParam);
            loacdPackingList.TargetViewType = ViewType.DetailView;
            loacdPackingList.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loacdPackingList.Shortcut = "CtrlU";

            loacdPackingList.CustomizePopupWindowParams += UpdateLocation_CustomizePopupWindowParams;
            loacdPackingList.Execute += UpdateLocation_Execute;
        }

        public void UpdateLocation_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var storageBinCode = ((DetailView)e.PopupWindowView).FindItem("StorageBinCode");
            var updateLocationParam = storageBinCode.CurrentObject as UpdateLocationParam;
            var updateLocationStorageDetailReports = updateLocationParam.Details;
            var location = updateLocationParam.StorageBinCode;

            MessageOptions options = null;
            if (!string.IsNullOrEmpty(location) && updateLocationStorageDetailReports.Any())
            {
                var connectString = Application.ConnectionString ?? string.Empty;
                using (var db = new QueryFactory(
                    new SqlConnection(connectString), new SqlServerCompiler()))
                {
                    var tableName = "StorageDetail";
                    var StorageDetailUpdateLocation = db.Query(tableName)
                        .WhereIn("ID", updateLocationStorageDetailReports.Select(s => s.ID).ToList())
                        .Update(new
                        {
                            StorageBinCode = location                           
                        });
                }
                options = Message.GetMessageOptions("Update location successfully. ", "Successs", InformationType.Success, null, 5000);
            }
            Application.ShowViewStrategy.ShowMessage(options);
            //
            foreach(var item in updateLocationStorageDetailReports)
            {
                item.StorageBinCode = location;
            }    
            //
            //var viewObject = View.CurrentObject as StorageDetailReportParam;
            //var service = new StorageDetailService();
            //var response = service.GetReport(viewObject.Customer?.ID ?? string.Empty, viewObject.Storage?.Code,
            //    viewObject.FromDate, viewObject.ToDate, viewObject.ProductionMethodCode?.Code).Result;

            //if (response.IsSuccess)
            //{
            //    var mapperConfig = new MapperConfiguration(c =>
            //    {
            //        c.CreateMap<StorageDetailReportResponseData, StorageDetailsReport>();
            //    });

            //    var mapper = mapperConfig.CreateMapper();

            //    viewObject.Details = response.Data
            //        .Select(x => mapper.Map<StorageDetailsReport>(x)).ToList();
            //    View.Refresh();
            //}
            //else
            //{
            //    var message = Message.GetMessageOptions(response.ErrorMessage, "Error",
            //        InformationType.Error, null, 5000);
            //    Application.ShowViewStrategy.ShowMessage(message);
            //}
            //
            View.Refresh();
        }

        private void UpdateLocation_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var viewObject = View.CurrentObject as StorageDetailReportParam;

            ListPropertyEditor listPropertyEditor = ((DetailView)View)
              .FindItem("Details") as ListPropertyEditor;

            var updateLocationStorageDetailReports = listPropertyEditor.ListView
                .SelectedObjects.Cast<StorageDetailsReport>().ToList();

            var model = new UpdateLocationParam();
            if(viewObject != null)
            {               
                model.Details = updateLocationStorageDetailReports;
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
