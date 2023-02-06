using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Response;
using System.Diagnostics;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class PurchaseReceivedSearchParam_ViewController : ObjectViewController<DetailView, PurchaseReceivedSearchParam>
    {
        public PurchaseReceivedSearchParam_ViewController()
        {
            InitializeComponent();
            SimpleAction loadReceiptAction = new SimpleAction(this, "LoadPurchaseReceived", PredefinedCategory.Unspecified);
            loadReceiptAction.ImageName = "Action_Search_Object_FindObjectByID";
            loadReceiptAction.Caption = "Search (Ctrl + L)";
            //loadReceiptAction.TargetObjectType = typeof(ReceiptReportParam);
            //loadReceiptAction.TargetViewType = ViewType.DetailView;
            loadReceiptAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadReceiptAction.Shortcut = "CtrlL";
            loadReceiptAction.Execute += LoadStorageDetailReportAction_Execute;
        }

        private void LoadStorageDetailReportAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as PurchaseReceivedSearchParam;
            var service = new PurchaseOrderService();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var response = service.GetReceived(viewObject.Customer?.ID ?? string.Empty, viewObject.Storage?.Code,
               viewObject.FromDate, viewObject.ToDate).Result;

            stopWatch.Stop();
            System.Diagnostics.Debug.WriteLine("Loading data from API: =>> " + stopWatch.ElapsedMilliseconds.ToString());


            if (response.IsSuccess)
            {
                var mapperConfig = new MapperConfiguration(c =>
                {
                    c.CreateMap<PurchaseReceivedDetailsReport, PurchaseReceivedDetails>();
                });

                var mapper = mapperConfig.CreateMapper();

                Stopwatch stopWatch1 = new Stopwatch();
                stopWatch1.Start();

                viewObject.Details = response.Data
                   .Select(x => mapper.Map<PurchaseReceivedDetails>(x)).ToList();

                

                View.Refresh();

                stopWatch1.Stop();
                System.Diagnostics.Debug.WriteLine("Mapping XAF: =>>> " + stopWatch1.ElapsedMilliseconds.ToString());
            }
            else
            {
                var message = Message.GetMessageOptions(response.ErrorMessage, "Error",
                    InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(message);
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
