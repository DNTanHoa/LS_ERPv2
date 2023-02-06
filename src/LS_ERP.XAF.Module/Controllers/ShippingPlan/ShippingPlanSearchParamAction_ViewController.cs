using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ultils.Extensions;
using Ultils.Helpers;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ShippingPlanSearchParamAction_ViewController 
        : ObjectViewController<DetailView, ShippingPlanSearchParam>
    {
        public ShippingPlanSearchParamAction_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction shippingPlanImportAction = new PopupWindowShowAction(this,
                "ShippingPlanImportAction", PredefinedCategory.Unspecified);
            shippingPlanImportAction.ImageName = "Import";
            shippingPlanImportAction.Caption = "Import Plan";
            shippingPlanImportAction.TargetObjectType = typeof(ShippingPlanSearchParam);
            shippingPlanImportAction.TargetViewType = ViewType.DetailView;
            shippingPlanImportAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            shippingPlanImportAction.CustomizePopupWindowParams += ShippingPlanImportAction_CustomizePopupWindowParams;
            shippingPlanImportAction.Execute += ShippingPlanImportAction_Execute;

            //
           
            //

            SimpleAction searchShippingPlan = new SimpleAction(this, "SearchShippingPlan", PredefinedCategory.Unspecified);
            searchShippingPlan.ImageName = "Action_Search_Object_FindObjectByID";
            searchShippingPlan.Caption = "Search (Ctrl + L)";
            searchShippingPlan.TargetObjectType = typeof(ShippingPlanSearchParam);
            searchShippingPlan.TargetViewType = ViewType.DetailView;
            searchShippingPlan.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchShippingPlan.Execute += SearchShippingPlan_Execute;
        }

        private void SearchShippingPlan_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as ShippingPlanSearchParam;
            var errorMessage = "";

            if (viewObject != null)
            {
                //var shippingPlans = new List<ShippingPlan>();
                try
                {
                    var customers = ObjectSpace.GetObjects<Customer>();
                    var connectionString = Application.ConnectionString ?? string.Empty;
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@CustomerID",viewObject?.Customer?.ID ?? string.Empty),
                        new SqlParameter("@Search",viewObject?.Search ?? string.Empty)
                    };

                    DataTable table = SqlHelper.FillByReader(connectionString, "sp_LoadShippingPlan", parameters);
                    var spl = table.AsListObject<ShippingPlan>();
                    viewObject.ShippingPlans = spl;
                    //foreach (DataRow dr in table.Rows)
                    //{
                    //    var shippingPlan = new ShippingPlan()
                    //    {
                    //       ID = (int)dr["ID"],
                    //       CustomerID = dr["CustomerID"].ToString(),
                    //       CompanyID = dr["CompanyID"].ToString(),
                    //       Title = dr["Title"].ToString(),
                    //       FilePath = dr["FilePath"].ToString(),
                    //       CreatedBy = dr["CreatedBy"].ToString(),
                    //       CreatedAt = (DateTime)dr["CreatedAt"],
                    //       LastUpdatedBy = dr[]
                    //    };
                    //    shippingPlan.Customer = customers.FirstOrDefault(x => x.ID == shippingPlan.CustomerID);

                    //    shippingPlans.Add(shippingPlan);
                    //}
                }
                catch (Exception ex)
                {
                    var error = Message.GetMessageOptions(errorMessage, "Error", InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(error);
                }

                //viewObject.ShippingPlans = shippingPlans;
            }

            View.Refresh();
        }

        private void ShippingPlanImportAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowViewCurrentObject as ImportShippingPlanParam;
            var service = new ShippingPlanService();
            var messageOptions = new MessageOptions();

            var request = new BulkShippingPlanRequest()
            {
                CustomerID = viewObject?.Customer?.ID,
                CompanyID = viewObject?.Company?.Code,
                UserName = SecuritySystem.CurrentUserName,
                Data = viewObject?.Details
            };
            var bulkResponse = service.BulkShippingPlan(request).Result;

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

        private void ShippingPlanImportAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new ImportShippingPlanParam()
            {
                Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(),
                Company = objectSpace.GetObjects<Company>().FirstOrDefault(),
            };
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
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
