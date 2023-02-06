using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using DevExpress.XtraReports.UI;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Report;
using LS_ERP.XAF.Module.Report.Issued;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class PrintIssued_ViewController : ViewController
    {
        public PrintIssued_ViewController()
        {
            InitializeComponent();

            SimpleAction printIssuedAction = new SimpleAction(this, "PrintIssuedAction", PredefinedCategory.Unspecified);
            printIssuedAction.ImageName = "Print";
            printIssuedAction.Caption = "Print (Ctrl + P)";
            printIssuedAction.TargetObjectType = typeof(Issued);
            printIssuedAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            printIssuedAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            printIssuedAction.Shortcut = "CtrlP";

            printIssuedAction.Execute += PrintIssuedAction_Execute;
        }

        private void PrintIssuedAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var issued = View.CurrentObject as Issued;

            var config = new MapperConfiguration(c =>
            {
                c.CreateMap<Issued, IssuedReportModel>()
                    .ForMember(x => x.IssuedNumber, y => y.MapFrom(s => s.Number))
                    .ForMember(x => x.IssuedDate, y => y.MapFrom(s => s.IssuedDate))
                    .ForMember(x => x.Issuer, y => y.MapFrom(s => s.CreatedBy))
                    .ForMember(x => x.Receiver, y => y.MapFrom(s => s.ReceivedBy))
                    .ForMember(x => x.Description, y => y.MapFrom(s => s.Description))
                    .ForMember(x => x.Details, y => y.MapFrom(s => s.IssuedGroupLines));
                c.CreateMap<IssuedGroupLine, IssuedReportDetailModel>()
                    .ForMember(x => x.Quantity, y => y.MapFrom(s => s.IssuedQuantity))
                    .ForMember(x => x.RequestQuantity, y => y.MapFrom(s => s.RequestQuantity))
                    .ForMember(x => x.UnitID, y => y.MapFrom(s => s.UnitID))
                    .ForMember(x => x.ItemCode, y => y.MapFrom(s => s.ItemID))
                    .ForMember(x => x.ItemColorCode, y => y.MapFrom(s => s.ItemColorCode))
                    .ForMember(x => x.ItemColorName, y => y.MapFrom(s => s.ItemColorName))
                    .ForMember(x => x.DyeLot, y => y.MapFrom(s => s.DyeLotNumber))
                    .ForMember(x => x.Roll, y => y.MapFrom(s => s.IssuedLines.Sum(l => l.Roll)))
                    .ForPath(x => x.IssuedReportModel.IssuedNumber, y => y.MapFrom(s => s.Issued.Number))
                    .ForPath(x => x.IssuedReportModel.IssuedDate, y => y.MapFrom(s => s.Issued.IssuedDate))
                    .ForPath(x => x.IssuedReportModel.Issuer, y => y.MapFrom(s => s.Issued.CreatedBy))
                    .ForPath(x => x.IssuedReportModel.Receiver, y => y.MapFrom(s => s.Issued.ReceivedBy))
                    .ForPath(x => x.IssuedReportModel.Description, y => y.MapFrom(s => s.Issued.Description))
                    .ForPath(x => x.IssuedReportModel.Details, y => y.MapFrom(s => s.Issued.IssuedGroupLines));
            });

            var mapper = config.CreateMapper();
            var reportModel = mapper.Map<IssuedReportModel>(issued);

            //int NoNum = 1;
            //foreach (var item in reportModel.Details)
            //{
            //    item.No = NoNum++.ToString();
            //}

            switch (issued.StorageCode)
            {
                case "AC":
                    var ACIssuedReport = new IssuedReport();
                    ACIssuedReport.ExportOptions.Xlsx.SheetName = issued.Number;
                    ACIssuedReport.DisplayName = issued.Number;
                    ACIssuedReport.DataSource = reportModel.Details;
                    ReportPrintTool ACIssuedPrintTool = new ReportPrintTool(ACIssuedReport);
                    ACIssuedPrintTool.ShowPreview();
                    break;
                case "FB":
                    {
                        switch (issued.CustomerID)
                        {
                            case "DE":
                                {
                                    var FBIssuedReportDE = new FBIssuedReportDE();
                                    FBIssuedReportDE.ExportOptions.Xlsx.SheetName = issued.Number;
                                    FBIssuedReportDE.DisplayName = issued.Number;
                                    FBIssuedReportDE.DataSource = reportModel.Details;
                                    ReportPrintTool FBIssuedPrintToolDE = new ReportPrintTool(FBIssuedReportDE);
                                    FBIssuedPrintToolDE.ShowPreview();
                                }
                                break;
                            default:
                                {
                                    var FBIssuedReport = new FBIssuedReport();
                                    FBIssuedReport.ExportOptions.Xlsx.SheetName = issued.Number;
                                    FBIssuedReport.DisplayName = issued.Number;
                                    FBIssuedReport.DataSource = reportModel.Details;
                                    ReportPrintTool FBIssuedPrintTool = new ReportPrintTool(FBIssuedReport);
                                    FBIssuedPrintTool.ShowPreview();
                                }
                                break;
                        }
                    }
                    break;
                default:
                    break;
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
