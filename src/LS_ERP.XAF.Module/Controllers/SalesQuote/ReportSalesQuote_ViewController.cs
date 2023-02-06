using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using DevExpress.XtraReports.UI;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Report;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ReportSalesQuote_ViewController : ViewController
    {
        public ReportSalesQuote_ViewController()
        {
            InitializeComponent();

            SimpleAction printSalesQuoteAction = new SimpleAction(this,
                "PrintSalesQuoteWithPreview",
                PredefinedCategory.Unspecified);
            printSalesQuoteAction.ImageName = "Task";
            printSalesQuoteAction.Caption = "Report";
            printSalesQuoteAction.TargetObjectType = typeof(SalesQuote);
            printSalesQuoteAction.TargetViewType = ViewType.Any;
            printSalesQuoteAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            printSalesQuoteAction.SelectionDependencyType =
                SelectionDependencyType.RequireSingleObject;

            printSalesQuoteAction.Execute += PrintSalesQuoteAction_Execute;
        }

        private void PrintSalesQuoteAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as SalesQuote;
            var defaultReport = new SalesQuote_Report();

            var dataSource = new List<SalesQuoteReportDetail>();

            foreach(var detail in viewObject.SalesQuoteDetails)
            {
                var reportDetail = new SalesQuoteReportDetail()
                {
                    ReportMaster = new SalesQuoteReport()
                    {
                        Image = viewObject.Image,
                        Season = viewObject.Season,
                        CustomerStyle = viewObject.CustomerStyle,
                        CostingDate = viewObject.CostingDate,
                        Gender = viewObject.Gender?.Name,
                        SizeRun = viewObject.SizeRun,
                        Description = viewObject.Description,
                        FactoryCode = viewObject.FactoryCode,
                        PrepareBy = viewObject.PrepareBy,
                        TargetFOBPrice = viewObject.TargetFOBPrice,
                        ApprovedBy = viewObject.ApprovedBy,
                        Labour = viewObject.Labour,
                        TestingFee = viewObject.TestingFee,
                        PriceTermCode = viewObject.PriceTermCode,
                        CMTPrice = viewObject.CMTPrice,
                        Discount = viewObject.Discount,
                    },
                    ItemID = detail.ItemID,
                    ItemName = detail.ItemName,
                    MaterialSortIndex = detail.MaterialType?.SortOrder ?? 0,
                    MaterialTypeCode = detail.MaterialType?.SortOrder?.ToString() + ". " +
                                        detail.MaterialType?.Name,
                    VendorID = detail.Vendor?.Name,
                    Position = detail.Position,
                    Amount = detail.Amount,
                    UnitID = detail.UnitID,
                    UnitPrice = detail.UnitPrice,
                    Country = detail.Vendor?.Country,
                    Note = detail.Note,
                    QuantityPerUnit = detail.QuantityPerUnit
                };

                dataSource.Add(reportDetail);
            }

            defaultReport.DataSource = dataSource;
            ReportPrintTool printTool = new ReportPrintTool(defaultReport);
            printTool.ShowPreview();
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
