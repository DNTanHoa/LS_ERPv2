using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using System;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class CopySalesQuote_ViewController : ViewController
    {
        public CopySalesQuote_ViewController()
        {
            InitializeComponent();

            SimpleAction copySalesQuote = new SimpleAction(this, 
                "CopySalesQuote",
                PredefinedCategory.Unspecified);
            copySalesQuote.ImageName = "Copy";
            copySalesQuote.Caption = "Copy";
            copySalesQuote.TargetObjectType = typeof(SalesQuote);
            copySalesQuote.TargetViewType = ViewType.Any;
            copySalesQuote.PaintStyle = 
                ActionItemPaintStyle.CaptionAndImage;
            copySalesQuote.SelectionDependencyType =
                SelectionDependencyType.RequireSingleObject;

            copySalesQuote.Execute += CopySalesQuote_Execute; ;
        }

        private void CopySalesQuote_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var salesQuote = View.CurrentObject as SalesQuote;

            var objectSpace = Application.CreateObjectSpace(typeof(SalesQuote));
            var newSalesQuote = objectSpace.CreateObject<SalesQuote>();
            newSalesQuote.SalesOrderID = salesQuote.SalesOrderID;
            newSalesQuote.CustomerID = salesQuote.CustomerID;
            newSalesQuote.DivisionID = salesQuote.DivisionID;
            newSalesQuote.CurrencyID = salesQuote.CurrencyID;
            newSalesQuote.CustomerStyle = salesQuote.CustomerStyle;
            newSalesQuote.Image = salesQuote.Image;
            newSalesQuote.Description = salesQuote.Description;
            newSalesQuote.TargetFOBPrice = salesQuote.TargetFOBPrice;
            newSalesQuote.CurrencyExchangeTypeID = salesQuote.CurrencyExchangeTypeID;
            newSalesQuote.PriceTermCode = salesQuote.PriceTermCode;
            newSalesQuote.FactoryCode = salesQuote.FactoryCode;
            newSalesQuote.SalesQuoteStatusCode = salesQuote.SalesQuoteStatusCode;
            newSalesQuote.Labour = salesQuote.Labour;
            newSalesQuote.Profit = salesQuote.Profit;
            newSalesQuote.TestingFee = salesQuote.TestingFee;
            newSalesQuote.CMTPrice = salesQuote.CMTPrice;
            newSalesQuote.Discount = salesQuote.Discount;
            newSalesQuote.SizeRun = salesQuote.SizeRun;
            newSalesQuote.Season = salesQuote.Season;
            newSalesQuote.GenderID = salesQuote.GenderID;
            newSalesQuote.Level = 0;
            newSalesQuote.SetCreateAudit(SecuritySystem.CurrentUserName);
            newSalesQuote.PrepareBy = SecuritySystem.CurrentUserName;
            newSalesQuote.CostingDate = DateTime.Now;

            if (salesQuote.SalesQuoteDetails.Any())
            {
                foreach(var salesQuoteDetail in salesQuote.SalesQuoteDetails)
                {
                    var newSalesQuoteDetail = new SalesQuoteDetail()
                    {
                        ExternalCode = salesQuoteDetail.ExternalCode,
                        ItemID = salesQuoteDetail.ItemID,
                        ItemName = salesQuoteDetail.ItemName,
                        Position = salesQuoteDetail.Position,
                        Consumption = salesQuoteDetail.Consumption,
                        UnitPrice = salesQuoteDetail.UnitPrice,
                        WastagePercent = salesQuoteDetail.WastagePercent,
                        UnitID = salesQuoteDetail.UnitID,
                        PriceUnitID = salesQuoteDetail.PriceUnitID,
                        VendorID = salesQuoteDetail.VendorID,
                        Amount = salesQuoteDetail.Amount,
                        Note = salesQuoteDetail.Note,
                        MaterialTypeCode = salesQuoteDetail.MaterialTypeCode,
                        Type = salesQuoteDetail.Type,
                    };

                    newSalesQuote.SalesQuoteDetails.Add(newSalesQuoteDetail);
                }
            }

            objectSpace.CommitChanges();
            View.Refresh(true);

            var messageOptions = Message.GetMessageOptions("Copy successfully", "Success",
                InformationType.Success, null, 5000);
            Application.ShowViewStrategy.ShowMessage(messageOptions);
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
