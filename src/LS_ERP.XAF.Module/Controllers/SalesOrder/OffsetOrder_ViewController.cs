using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class OffsetOrder_ViewController : ViewController
    {
        public OffsetOrder_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction offsetSalesOrder = new PopupWindowShowAction(this,
                "OffsetSalesOrder", PredefinedCategory.Unspecified);
            offsetSalesOrder.ImageName = "Offset";
            offsetSalesOrder.Caption = "Offset";
            offsetSalesOrder.TargetObjectType = typeof(SalesOrder);
            offsetSalesOrder.TargetViewType = ViewType.ListView;
            offsetSalesOrder.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            offsetSalesOrder.CustomizePopupWindowParams += 
                OffsetSalesOrder_CustomizePopupWindowParams;
            offsetSalesOrder.Execute += OffsetSalesOrder_Execute;
        }

        private void OffsetSalesOrder_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var service = new SalesOrderService();
            var param = e.PopupWindowViewCurrentObject as SalesOrderOffsetParam;
            var messageOptions = new MessageOptions();

            if (param != null)
            {
                var mapperConfig = new MapperConfiguration(c =>
                {
                    c.CreateMap<SalesOrderOffsetMaterial, SalesOrderOffset>();
                });
                var mapper = mapperConfig.CreateMapper();

                var request = new OffsetSalesOrderRequest()
                {
                    CustomerID = param.Customer.ID,
                    UserName = SecuritySystem.CurrentUserName,
                    Data = param.OffsetDetails.Select(x => mapper.Map<SalesOrderOffset>(x)).ToList()
                };

                var response = service.SalesOrderOffset(request).Result;

                if (response != null)
                {
                    if (response.Result.Code == "000")
                    {
                        messageOptions = Message.GetMessageOptions(response.Result.Message, "Success",
                            InformationType.Success, null, 5000);
                    }
                    else
                    {
                        messageOptions = Message.GetMessageOptions(response.Result.Message, "Error",
                            InformationType.Error, null, 5000);
                    }
                }
                else
                {
                    messageOptions = Message
                        .GetMessageOptions("Unexpected error", "Error", InformationType.Error, null, 5000);
                }

                View.Refresh();
                Application.ShowViewStrategy.ShowMessage(messageOptions);
            }
        }

        private void OffsetSalesOrder_CustomizePopupWindowParams(object sender, 
            CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var model = new SalesOrderOffsetParam();
            e.Maximized = true;
            e.DialogController.SaveOnAccept = false;
            var view = Application.CreateDetailView(objectSpace,model,false);
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
