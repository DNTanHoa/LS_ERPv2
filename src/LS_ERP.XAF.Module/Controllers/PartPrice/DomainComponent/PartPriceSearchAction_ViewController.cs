using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ultils.Helpers;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class PartPriceSearchAction_ViewController : ObjectViewController<DetailView, PartPriceSearchParam>
    {
        public PartPriceSearchAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchPartPrice = new SimpleAction(this, "SearchPartPrice", PredefinedCategory.Unspecified);
            searchPartPrice.ImageName = "Action_Search_Object_FindObjectByID";
            searchPartPrice.Caption = "Search (Ctrl + L)";
            searchPartPrice.TargetObjectType = typeof(PartPriceSearchParam);
            searchPartPrice.TargetViewType = ViewType.DetailView;
            searchPartPrice.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            searchPartPrice.Shortcut = "CtrlL";

            searchPartPrice.Execute += SearchPartPrice_Execute;

            PopupWindowShowAction importPartPrice = new PopupWindowShowAction(this,
                "ImportPartPrice", PredefinedCategory.Unspecified);
            importPartPrice.ImageName = "Import";
            importPartPrice.Caption = "Import (Ctrl + I)";
            importPartPrice.TargetObjectType = typeof(PartPriceSearchParam);
            importPartPrice.TargetViewType = ViewType.DetailView;
            importPartPrice.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            importPartPrice.Shortcut = "CtrlI";

            importPartPrice.CustomizePopupWindowParams += ImportPartPriceAction_CustomizePopupWindowParams;
            importPartPrice.Execute += ImportPartPriceAction_Execute;
        }
        private void SearchPartPrice_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PartPriceSearchParam;
            var errorMessage = "";

            if (viewObject != null)
            {
                var partMasters = new List<PartPrice>();
                try
                {
                    var connectionString = Application.ConnectionString ?? string.Empty;
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@CustomerID",viewObject?.Customer?.ID ?? string.Empty),
                        new SqlParameter("@Search",viewObject?.Search ?? string.Empty)
                    };

                    DataTable table = SqlHelper.FillByReader_ItemMasterJob(connectionString, "sp_LoadPartPrice", parameters);
                    foreach (DataRow dr in table.Rows)
                    {
                        var partPrice = new PartPrice()
                        {
                            ID = (int)dr["ID"],
                            StyleNO = dr["StyleNO"].ToString(),
                            GarmentColorCode = dr["GarmentColorCode"].ToString(),
                            Season = dr["Season"].ToString(),
                            EffectiveDate = (DateTime)dr["EffectiveDate"],
                            ExpiryDate = (DateTime)dr["ExpiryDate"],
                            ProductionType = dr["ProductionType"].ToString(),
                            CustomerID = dr["CustomerID"].ToString(),
                            Price = (decimal)dr["Price"]
                        };

                        partMasters.Add(partPrice);
                    }
                }
                catch (Exception ex)
                {
                    var error = Message.GetMessageOptions(errorMessage, "Error", InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(error);
                }

                viewObject.PartPrices = partMasters;
            }

            View.Refresh();
        }
        private async void ImportPartPriceAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowViewCurrentObject as ImportPartPriceParam;
            var service = new PartPriceService();
            var messageOptions = new MessageOptions();

            var request = new ImportPartPriceRequest()
            {
                CustomerID = viewObject?.Customer?.ID,
                UserName = SecuritySystem.CurrentUserName,
                FilePath = viewObject?.FilePath,    
            };
            var importResponse = await service.ImportPartPrice(request);

            if (importResponse != null)
            {
                if (importResponse.Result.Code=="000")
                {
                    messageOptions = Message.GetMessageOptions("Import successfully", "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    messageOptions = Message.GetMessageOptions(importResponse.Result.Message, "Error",
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

        private void ImportPartPriceAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new ImportPartPriceParam()
            {
                Customer = objectSpace.GetObjects<Customer>().FirstOrDefault(),
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
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
