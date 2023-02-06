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

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class BoxInfoSearchParamAction_ViewController : ObjectViewController<DetailView, BoxInfoSearchParam>
    {
        public BoxInfoSearchParamAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchBoxInfo = new SimpleAction(this, "SearchBoxInfo", PredefinedCategory.Unspecified);
            searchBoxInfo.ImageName = "Action_Search_Object_FindObjectByID";
            searchBoxInfo.Caption = "Search (Ctrl + L)";
            searchBoxInfo.TargetObjectType = typeof(BoxInfoSearchParam);
            searchBoxInfo.TargetViewType = ViewType.DetailView;
            searchBoxInfo.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchBoxInfo.Execute += SearchBoxInfo_Execute;
        }
        private void SearchBoxInfo_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as BoxInfoSearchParam;
            //var customers = ObjectSpace.GetObjects<Customer>();
            var errorMessage = "";

            if (viewObject != null)
            {
                var boxInfos = new List<BoxInfo>();
                try
                {
                    var connectionString = Application.ConnectionString ?? string.Empty;
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@CustomerID",viewObject?.Customer?.ID ?? string.Empty),
                        new SqlParameter("@Search",viewObject?.Search ?? string.Empty)
                    };

                    DataTable table = SqlHelper.FillByReader_ItemMasterJob(connectionString, "sp_LoadBoxInfo", parameters);
                    foreach (DataRow dr in table.Rows)
                    {
                        var boxInfo = new BoxInfo()
                        {
                            ID = (long)dr["ID"],
                            TagID = dr["TagID"].ToString(),
                            GarmentColorCode = dr["GarmentColorCode"].ToString(),
                            ItemCode = dr["ItemCode"].ToString(),
                            Description = dr["Description"].ToString(),
                            GarmentSize = dr["GarmentSize"].ToString(),
                            CustomerStyle = dr["CustomerStyle"].ToString(),
                            QuantityPerBox = (decimal)dr["QuantityPerBox"],
                            CustomerID = dr["CustomerID"].ToString(),
                        };
                        if (boxInfo.ID == 0)
                        {
                            boxInfo.SetCreateAudit(SecuritySystem.CurrentUserName);
                        }
                        else
                        {
                            boxInfo.SetUpdateAudit(SecuritySystem.CurrentUserName);
                        }

                        boxInfos.Add(boxInfo);
                    }
                }
                catch (Exception ex)
                {
                    var error = Message.GetMessageOptions(errorMessage, "Error", InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(error);
                }

                viewObject.BoxInfos = boxInfos;
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
