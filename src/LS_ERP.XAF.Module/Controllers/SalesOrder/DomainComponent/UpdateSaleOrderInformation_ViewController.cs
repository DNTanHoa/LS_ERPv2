using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class UpdateSaleOrderInformation_ViewController : ViewController
    {
        public UpdateSaleOrderInformation_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction updateSalesOrderInfor = new PopupWindowShowAction(this, "UpdateSalesOrderInfor", PredefinedCategory.Unspecified);
            updateSalesOrderInfor.ImageName = "Highlight";
            updateSalesOrderInfor.TargetObjectType = typeof(SalesOrderSearchParam);
            updateSalesOrderInfor.TargetViewType = ViewType.DetailView;
            updateSalesOrderInfor.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            updateSalesOrderInfor.CustomizePopupWindowParams += UpdateSalesOrderInfor_CustomizePopupWindowParams;
            updateSalesOrderInfor.Execute += UpdateSalesOrderInfor_Execute; ;
        }

        private void UpdateSalesOrderInfor_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var viewObject = e.PopupWindowViewCurrentObject as SalesOrderUpdateInformationParam;
            MessageOptions message = null;

            if (viewObject != null)
            {
                var service = new ItemStyleService();
                var request = new BulkUpdateInforRequest()
                {
                    UserName = SecuritySystem.CurrentUserName,
                    Data = viewObject.Data.Select(x => new Dtos.ItemStyleInfoDto() 
                    { 
                        LSStyle = x.LSStyle,
                        IssuedDate = x.IssuedDate,
                        AccessoriesDate = x.AccessoriesDate,
                        FabricDate = x.FabricDate,
                        ProductionSketDeliveryDate = x.ProductionSketDeliveryDate
                        
                    }).ToList()
                };

                var response = service.BulkUpdateInfor(request).Result;

                if (response.Result.Code == "000")
                {
                    message = Message.GetMessageOptions(response.Result.Message, "Successfully",
                        InformationType.Success, null, 5000);
                    View.Refresh();
                }
                else
                {
                    message = Message.GetMessageOptions(response.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }
            else
            {
                message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error",
                    InformationType.Error, null, 5000);
            }

            if (message != null)
                Application.ShowViewStrategy.ShowMessage(message);
        }

        private void UpdateSalesOrderInfor_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new SalesOrderUpdateInformationParam();
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
