using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Process;
using SqlKata.Compilers;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class LoadPackingListFromShippingPlan_ViewController : ObjectViewController<DetailView, Invoice>
    {
        public LoadPackingListFromShippingPlan_ViewController()
        {
            InitializeComponent();

            SimpleAction LoadPackingListFromShippingPlan = new SimpleAction(this, "LoadPackingListFromShippingPlan", PredefinedCategory.Unspecified);
            LoadPackingListFromShippingPlan.ImageName = "Actions_Reload";
            LoadPackingListFromShippingPlan.Caption = "Load PackingList From ShippingPlan";            
            LoadPackingListFromShippingPlan.PaintStyle = ActionItemPaintStyle.CaptionAndImage;         

            LoadPackingListFromShippingPlan.Execute += LoadPackingListFromShippingPlan_Execute;
        }

        public virtual void LoadPackingListFromShippingPlan_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var invoice = View.CurrentObject as Invoice;
            var objectSpace = this.ObjectSpace;
            var ifgUnit = ObjectSpace.GetObjectByKey<Unit>("DZ");

            var partPrices = new List<PartPrice>();
            var salesOrders = new List<SalesOrder>();
            var tableName = "";
            var filter = "";

            if (string.IsNullOrEmpty(tableName))
                tableName = typeof(PartPrice).Name;

            var connectString = Application.ConnectionString ?? string.Empty;
            var db = new QueryFactory(new SqlConnection(connectString), new SqlServerCompiler());

            filter = " [CustomerID] = '" + invoice?.CustomerID + "'";
                    //AND"
                    //+ " CAST([EffectiveDate] AS DATE) <= '" + invoice?.Date.Value.Date + "' AND"
                    //+ " CAST([ExpiryDate] AS DATE) >= '" + invoice?.Date.Value.Date + "'";

            partPrices = db.Query(tableName)
                    .WhereRaw(filter).Get<PartPrice>().ToList();

            var messageOptions = new MessageOptions();
            string errorMessage = string.Empty;
            if (invoice!=null)
            {
                if(invoice.IsConfirmed == true)
                {                   
                    return;
                }    
                if(invoice.ID != 0)
                {
                    if(!string.IsNullOrEmpty(invoice.Code))
                    {
                        // get PKL
                        var listShippingPlanDetail = objectSpace.GetObjects<ShippingPlanDetail>().Where(s => s.InvoiceNumber == invoice.Code
                                                                                                        && IsNotExistInPackingListOfInvoice(invoice,s)
                                                                                                        ).ToList();                        
                        var packingList = objectSpace.GetObjects<PackingList>().Where(p=>listShippingPlanDetail.Select(s=>s.PackingListID).ToList().Contains(p.ID)).ToList();

                        if(packingList.Any())
                        {
                            var styles = packingList.SelectMany(x => x.ItemStyles).ToList();

                            filter = " [ID] IN (" + string.Join(",", styles.Select(x => "'" + x.SalesOrderID + "'")) + ")";

                            salesOrders = db.Query("SalesOrders")
                                    .WhereRaw(filter).Get<SalesOrder>().ToList();
                        }    
                        // 
                        InvoiceProcessor.CreateOrUpdateInvoiceDetail(invoice, packingList, SecuritySystem.CurrentUserName, ifgUnit, partPrices, salesOrders, out errorMessage);

                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            messageOptions = Message.GetMessageOptions(errorMessage, "Error", InformationType.Error, null, 5000);
                        }
                        ObjectSpace.CommitChanges();
                        View.Refresh();                      
                    }    
                }    
            }          
        }
        public Boolean IsNotExistInPackingListOfInvoice( Invoice invoice,ShippingPlanDetail shippingPlanDetail)
        {            
            var existPKL = invoice?.PackingList?.Where(p => p.ID == shippingPlanDetail.PackingListID).FirstOrDefault();
            if (existPKL != null)
                return false;
            return true;            
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
