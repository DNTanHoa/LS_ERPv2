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
    public partial class LoadingPlanSearchParamAction_ViewController : ObjectViewController<DetailView, LoadingPlanSearchParam>
    {
        public LoadingPlanSearchParamAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchLoadingPlan = new SimpleAction(this, "SearchLoadingPlan", PredefinedCategory.Unspecified);
            searchLoadingPlan.ImageName = "Action_Search_Object_FindObjectByID";
            searchLoadingPlan.Caption = "Search (Ctrl + L)";
            searchLoadingPlan.TargetObjectType = typeof(LoadingPlanSearchParam);
            searchLoadingPlan.TargetViewType = ViewType.DetailView;
            searchLoadingPlan.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchLoadingPlan.Execute += SearchLoadingPlan_Execute;
        }
        private void SearchLoadingPlan_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as LoadingPlanSearchParam;
            //var customers = ObjectSpace.GetObjects<Customer>();
            var errorMessage = "";

            if (viewObject != null)
            {
                var loadingPlans = new List<LoadingPlan>();
                try
                {
                    var connectionString = Application.ConnectionString ?? string.Empty;
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@CustomerID",viewObject?.Customer?.ID ?? string.Empty),
                        new SqlParameter("@ContainerNumber",viewObject?.ContainerNumber ?? string.Empty),
                        new SqlParameter("@OrderNumber",viewObject?.OrderNumber ?? string.Empty)
                    };

                    DataTable table = SqlHelper.FillByReader_ItemMasterJob(connectionString, "sp_LoadLoadingPlan", parameters);
                    foreach (DataRow dr in table.Rows)
                    {
                        var loadingPlan = new LoadingPlan()
                        {
                            ID = (long)dr["ID"],
                            ContainerNumber = dr["ContainerNumber"].ToString(),
                            ASNumber = dr["ASNumber"].ToString(),
                            TiersName = dr["TiersName"].ToString(),
                            Shu = dr["Shu"].ToString(),
                            OrderNumber = dr["OrderNumber"].ToString(),
                            ItemCode = dr["ItemCode"].ToString(),
                            PCB = (decimal)dr["PCB"],
                            Port = dr["Port"].ToString(),
                            Rank = (int)dr["Rank"],
                            ORINumber = dr["ORINumber"].ToString(),
                            GrossWeight = (decimal)dr["GrossWeight"],
                            NetWeight = (decimal)dr["NetWeight"],
                            Quantity = (decimal)dr["Quantity"],
                            ModelCode = dr["ModelCode"].ToString(),
                            Destination = dr["Destination"].ToString(),
                            Volumn = (decimal)dr["Volumn"],
                            Description = dr["Description"].ToString(),
                            CustomerID = dr["CustomerID"].ToString(),
                        };
                        if (loadingPlan.ID == 0)
                        {
                            loadingPlan.SetCreateAudit(SecuritySystem.CurrentUserName);
                        }
                        else
                        {
                            loadingPlan.SetUpdateAudit(SecuritySystem.CurrentUserName);
                        }

                        loadingPlans.Add(loadingPlan);
                    }
                }
                catch (Exception ex)
                {
                    var error = Message.GetMessageOptions(errorMessage, "Error", InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(error);
                }

                viewObject.LoadingPlans = loadingPlans;
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
