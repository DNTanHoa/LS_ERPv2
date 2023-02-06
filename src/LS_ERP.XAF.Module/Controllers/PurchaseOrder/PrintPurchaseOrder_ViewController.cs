using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using DevExpress.XtraReports.UI;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.BusinessObjects;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Report;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class PrintPurchaseOrder_ViewController : ViewController
    {
        public PrintPurchaseOrder_ViewController()
        {
            InitializeComponent();

            SimpleAction printPurchaseOrderWithPreviewAction = new SimpleAction(this,
                "PrintPurchaseOrderWithPreview",
                PredefinedCategory.Unspecified);
            printPurchaseOrderWithPreviewAction.ImageName = "Task";
            printPurchaseOrderWithPreviewAction.Caption = "Report";
            printPurchaseOrderWithPreviewAction.TargetObjectType = typeof(PurchaseOrder);
            printPurchaseOrderWithPreviewAction.TargetViewType = ViewType.Any;
            printPurchaseOrderWithPreviewAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            printPurchaseOrderWithPreviewAction.SelectionDependencyType =
                SelectionDependencyType.RequireSingleObject;

            printPurchaseOrderWithPreviewAction.Execute += PrintPurchaseOrderWithPreviewActionAction_Execute;
        }

        private void PrintPurchaseOrderWithPreviewActionAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var purchaseOrder = View.CurrentObject as PurchaseOrder;
            var objectSpace = Application.CreateObjectSpace(typeof(PurchaseOrder));
            purchaseOrder = objectSpace.GetObjectByKey<PurchaseOrder>(purchaseOrder.ID);

            var config = new MapperConfiguration(c =>
            {
                c.CreateMap<PurchaseOrderGroupLine, PurchaseOrderReportDetail>()
                .ForMember(x => x.UnitID, y => y.MapFrom(s => s.UnitID))
                .ForMember(x => x.RoundUp, y => y.MapFrom(s => s.Unit.RoundUp))
                .ForMember(x => x.RoundDown, y => y.MapFrom(s => s.Unit.RoundDown));

                c.CreateMap<PurchaseOrder, PurchaseOrderReport>()
                    .ForMember(x => x.CompanyName, y => y.MapFrom(s => s.Company.Name))
                    .ForMember(x => x.CompanyAddress, y => y.MapFrom(s => s.Company.DisplayAddress))
                    .ForMember(x => x.CompanyPhoneNumber, y => y.MapFrom(s => s.Company.DisplayPhone))
                    .ForMember(x => x.CompanyFaxNumber, y => y.MapFrom(s => s.Company.DisplayFaxNumber))
                    .ForMember(x => x.VenderAddress, y => y.MapFrom(s => s.Vendor.Address))
                    .ForMember(x => x.VenderName, y => y.MapFrom(s => s.Vendor.Name))
                    .ForMember(x => x.DeliveryDate, y => y.MapFrom(s => s.EstimateShipDate))
                    .ForMember(x => x.PaymentTerm, y => y.MapFrom(s => s.PaymentTerm.Description))
                    .ForMember(x => x.ShippingTerm, y => y.MapFrom(s => s.ShippingTerm.Description))
                    .ForMember(x => x.ShippingMethod, y => y.MapFrom(s => s.ShippingMethod.Name))
                    .ForMember(x => x.PurchaseOrderDate, y => y.MapFrom(s => s.OrderDate))
                    .ForMember(x => x.PurchaseNumber, y => y.MapFrom(s => s.Number))
                    .ForMember(x => x.Currency, y => y.MapFrom(s => s.CurrencyID))
                    .ForMember(x => x.Details, y => y.MapFrom(s => s.PurchaseOrderGroupLines))
                    .ForMember(x => x.CurrencyExhangeValue, y => y.MapFrom(s => s.CurrencyExchangeValue));
            });

            var mapper = config.CreateMapper();
            var contact = ObjectSpace.GetObjects<ApplicationUser>(CriteriaOperator.Parse("UserName = ?", purchaseOrder.CreatedBy))
                .FirstOrDefault();
            var supervisor = ObjectSpace.GetObjects<ApplicationUser>(CriteriaOperator.Parse("UserName = ?", contact.Supervisor))
                .FirstOrDefault();

            var purchaseOrderReport = mapper.Map<PurchaseOrderReport>(purchaseOrder);
            int rounding = purchaseOrder.Currency?.Rounding ?? 0;

            purchaseOrderReport.ContactPerson = contact.FullName ?? "";
            purchaseOrderReport.ContactPhone = contact.Phone ?? "";
            purchaseOrderReport.ContactEmail = contact.Email ?? "";
            purchaseOrderReport.MerchandiserSignature = contact.Signature ?? "";
            purchaseOrderReport.HeadOfDepartmentSignature = supervisor?.Signature ?? "";

            decimal exchangeRate = 1;

            if (!string.IsNullOrEmpty(purchaseOrderReport.CurrencyExchangeType))
            {
                string startCurrency = purchaseOrderReport.CurrencyExchangeType.Substring(0, 3);
                if (purchaseOrderReport.Currency != startCurrency
                    && purchaseOrderReport.CurrencyExhangeValue != null
                    && purchaseOrderReport.CurrencyExhangeValue != 0)
                {
                    exchangeRate = (decimal)purchaseOrderReport.CurrencyExhangeValue;
                }
            }

            Dictionary<string, PurchaseOrderReportDetail> newDicReportDetail =
                new Dictionary<string, PurchaseOrderReportDetail>();

            if (purchaseOrder != null)
            {
                foreach (var itemDetail in purchaseOrderReport.Details)
                {

                    string key = itemDetail.ToDictionaryKey(purchaseOrder.CustomerID);

                    if (newDicReportDetail.TryGetValue(key,
                        out PurchaseOrderReportDetail purchaseOrderReportDetail))
                    {
                        purchaseOrderReportDetail.Quantity +=
                            (decimal)itemDetail.Quantity;

                        newDicReportDetail[key] =
                            purchaseOrderReportDetail;
                    }
                    else
                    {
                        newDicReportDetail[key] = itemDetail;
                    }
                }
            }

            newDicReportDetail.Values.ToList()
                .ForEach((x) =>
                {
                    x.PurchaseOrderReport = purchaseOrderReport;
                    x.Price = x.Price * exchangeRate;
                    x.Quantity = x.RoundQuantity(x);
                    x.Total = x.Price * x.Quantity;
                });

            purchaseOrderReport.TotalQuantity = newDicReportDetail.Values.ToList().Sum(x => x.Quantity);

            int no = 1;
            newDicReportDetail.Values.ToList().GroupBy(x => x.ItemID).ToList()
                .ForEach((g) =>
                {
                    foreach (var item in g)
                    {
                        item.No = no.ToString();
                        no += 1;
                    }
                });

            purchaseOrderReport.SubTotal = newDicReportDetail.Values.ToList().Sum(x => x.Total);
            purchaseOrderReport.SubTotalText = purchaseOrderReport.SubTotal?.ToString("N" + rounding.ToString());
            if (purchaseOrder.Tax != null)
            {
                purchaseOrderReport.Vat = decimal.Parse(purchaseOrder.Tax?.Value);
                purchaseOrderReport.VatText = purchaseOrderReport.Vat?.ToString("N" + rounding.ToString());

                purchaseOrderReport.TotalVat =
                    decimal.Parse(purchaseOrder.Tax?.Value) * purchaseOrderReport.SubTotal / 100;
                purchaseOrderReport.TotalVatText = purchaseOrderReport.TotalVat?.ToString("N" + rounding.ToString());
            }

            purchaseOrderReport.Total = purchaseOrderReport.SubTotal + purchaseOrderReport.TotalVat;
            purchaseOrderReport.TotalText = purchaseOrderReport.Total?.ToString("N" + rounding.ToString());

            if (purchaseOrder != null)
            {
                switch (purchaseOrder.CustomerID)
                {
                    case "DE":
                        purchaseOrderReport.Details = newDicReportDetail.Values.ToList();
                        var DEReport = new PurchaseOrder_DE_Report();
                        DEReport.DataSource = purchaseOrderReport.Details;
                        DEReport.ExportOptions.Xlsx.SheetName = purchaseOrder.Number;
                        DEReport.DisplayName = purchaseOrder.Number;
                        ReportPrintTool DEPrintTool = new ReportPrintTool(DEReport);
                        DEPrintTool.ShowPreview();
                        break;
                    case "HA":
                        purchaseOrderReport.Details = newDicReportDetail.Values.ToList();
                        var HAReport = new PurchaseOrder_HA_Report();
                        HAReport.DataSource = purchaseOrderReport.Details;
                        HAReport.ExportOptions.Xlsx.SheetName = purchaseOrder.Number;
                        HAReport.DisplayName = purchaseOrder.Number;
                        ReportPrintTool HAPrintTool = new ReportPrintTool(HAReport);
                        HAPrintTool.ShowPreview();
                        break;
                    default:
                        purchaseOrderReport.Details = newDicReportDetail.Values.ToList();
                        var defaultReport = new PurchaseOrder_Default_Report();
                        defaultReport.DataSource = purchaseOrderReport.Details;
                        defaultReport.ExportOptions.Xlsx.SheetName = purchaseOrder.Number;
                        defaultReport.DisplayName = purchaseOrder.Number;
                        ReportPrintTool printTool = new ReportPrintTool(defaultReport);
                        printTool.ShowPreview();
                        break;
                }
            }
            else
            {
                var error = Message.GetMessageOptions("Unknown error, contact your admin.", "Error",
                    InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(error);
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
