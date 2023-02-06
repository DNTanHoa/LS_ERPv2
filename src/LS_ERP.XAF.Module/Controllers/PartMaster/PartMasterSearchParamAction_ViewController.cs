using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ultils.Helpers;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class PartMasterSearchParamAction_ViewController
        : ObjectViewController<DetailView, PartMasterSearchParam>
    {
        public PartMasterSearchParamAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchPartMaster = new SimpleAction(this, "SearchPartMaster", PredefinedCategory.Unspecified);
            searchPartMaster.ImageName = "Action_Search_Object_FindObjectByID";
            searchPartMaster.Caption = "Search (Ctrl + L)";
            searchPartMaster.TargetObjectType = typeof(PartMasterSearchParam);
            searchPartMaster.TargetViewType = ViewType.DetailView;
            searchPartMaster.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchPartMaster.Execute += SearchPartMaster_Execute;

            PopupWindowShowAction syncPartDecathlon = new PopupWindowShowAction(this, "SyncPartDecathlon", PredefinedCategory.Unspecified);
            syncPartDecathlon.ImageName = "Action_Search_Object_FindObjectByID";
            syncPartDecathlon.Caption = "Sync Decathlon";
            syncPartDecathlon.TargetObjectType = typeof(PartMasterSearchParam);
            syncPartDecathlon.TargetViewType = ViewType.DetailView;
            syncPartDecathlon.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            syncPartDecathlon.CustomizePopupWindowParams += 
                SyncPartDecathlon_CustomizePopupWindowParams;
            syncPartDecathlon.Execute += SyncPartDecathlon_Execute;
        }

        private void SyncPartDecathlon_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            
        }

        private void SyncPartDecathlon_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new PartMasterSyncParam();
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = view;
        }

        private void SearchPartMaster_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PartMasterSearchParam;
            var errorMessage = "";

            if (viewObject != null)
            {
                var partMasters = new List<PartMaster>();
                try
                {
                    var connectionString = Application.ConnectionString ?? string.Empty;
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@CustomerID",viewObject?.Customer?.ID ?? string.Empty),
                        new SqlParameter("@Search",viewObject?.Search ?? string.Empty)
                    };

                    DataTable table = SqlHelper.FillByReader_ItemMasterJob(connectionString, "sp_LoadPartMaster", parameters);
                    foreach (DataRow dr in table.Rows)
                    {
                        var partMaster = new PartMaster()
                        {
                            ID = dr["ID"].ToString(),
                            ExternalID = dr["ExternalID"].ToString(),
                            GarmentColorCode = dr["GarmentColorCode"].ToString(),
                            GarmentColorName = dr["GarmentColorName"].ToString(),
                            GarmentSize = dr["GarmentSize"].ToString(),
                            Season = dr["Season"].ToString(),
                            CustomerStyle = dr["CustomerStyle"].ToString(),
                            CustomerID = dr["CustomerID"].ToString(),
                        };

                        partMasters.Add(partMaster);
                    }
                }
                catch (Exception ex)
                {
                    var error = Message.GetMessageOptions(errorMessage, "Error", InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(error);
                }

                viewObject.PartMasters = partMasters;
            }

            View.Refresh();
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
