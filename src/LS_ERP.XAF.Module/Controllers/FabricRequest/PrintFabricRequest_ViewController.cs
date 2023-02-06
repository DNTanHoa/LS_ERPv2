using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using DevExpress.XtraReports.UI;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Report;
using LS_ERP.XAF.Module.Report.FabricRequest;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class PrintFabricRequest_ViewController : ObjectViewController<ListView, FabricRequest>
    {
        public PrintFabricRequest_ViewController()
        {
            InitializeComponent();
            SimpleAction printIssuedAction = new SimpleAction(this, "PrintFabricRequestAction", PredefinedCategory.Unspecified);
            printIssuedAction.ImageName = "Print";
            printIssuedAction.Caption = "Print (Ctrl + P)";
            //printIssuedAction.TargetObjectType = typeof(Issued);
            printIssuedAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            printIssuedAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            printIssuedAction.Shortcut = "CtrlP";

            printIssuedAction.Execute += PrintFabricRequestAction_Execute;
        }

        private void PrintFabricRequestAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var fabricRequest = View.CurrentObject as FabricRequest;

            var config = new MapperConfiguration(c =>
            {
                c.CreateMap<FabricRequest, FBRequestReportModel>()
                    //.ForMember(x => x.IssuedNumber, y => y.MapFrom(s => s.Number))
                    //.ForMember(x => x.IssuedDate, y => y.MapFrom(s => s.IssuedDate))
                    //.ForMember(x => x.Issuer, y => y.MapFrom(s => s.CreatedBy))
                    //.ForMember(x => x.Receiver, y => y.MapFrom(s => s.ReceivedBy))
                    //.ForMember(x => x.Description, y => y.MapFrom(s => s.Description))
                    //.ForMember(x => x.Details, y => y.MapFrom(s => s.IssuedGroupLines))
                    ;
                c.CreateMap<FabricRequestDetail, FBRequestReportDetailModel>()
                    .ForPath(x => x.OrderNumber, y => y.MapFrom(s => s.FabricRequest.OrderNumber))
                    .ForPath(x => x.CustomerStyleNumber, y => y.MapFrom(s => s.FabricRequest.CustomerStyleNumber))
                    .ForPath(x => x.FBRequestReportModel.RequestDate, y => y.MapFrom(s => s.FabricRequest.RequestDate))
                    .ForPath(x => x.FBRequestReportModel.CompanyShortName, y => y.MapFrom(s => s.FabricRequest.Company.ShortName))
                    .ForPath(x => x.FBRequestReportModel.Reason, y => y.MapFrom(s => s.FabricRequest.Reason))
                    ;
            });

            var mapper = config.CreateMapper();
            var reportModel = mapper.Map<FBRequestReportModel>(fabricRequest);

            int NoNum = 1;
            foreach (var item in reportModel.Details)
            {
                item.No = NoNum++.ToString();
            }

            var FBRequestReports = new FBRequestReport();
            FBRequestReports.ExportOptions.Xlsx.SheetName = reportModel.CustomerStyleNumber;
            FBRequestReports.DisplayName = reportModel.CustomerStyleNumber;
            FBRequestReports.DataSource = reportModel.Details;
            ReportPrintTool FBRequestPrintTool = new ReportPrintTool(FBRequestReports);
            FBRequestPrintTool.ShowPreview();
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
