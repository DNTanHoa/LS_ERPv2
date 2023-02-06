using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using System;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class SubmitSalesQuote_ViewController : ViewController
    {
        public SubmitSalesQuote_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction submitSalesQuoteAction = new PopupWindowShowAction(this, "SubmitSalesQuote",
                PredefinedCategory.Unspecified);
            submitSalesQuoteAction.ImageName = "Actions_Send";
            submitSalesQuoteAction.Caption = "Submit";
            submitSalesQuoteAction.TargetObjectType = typeof(SalesQuote);
            submitSalesQuoteAction.TargetViewType = ViewType.Any;
            submitSalesQuoteAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            submitSalesQuoteAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;

            submitSalesQuoteAction.CustomizePopupWindowParams += SubmitSalesQuoteAction_CustomizePopupWindowParams;
            submitSalesQuoteAction.Execute += SubmitSalesQuoteAction_Execute;
        }

        private void SubmitSalesQuoteAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            
        }

        private void SubmitSalesQuoteAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            throw new NotImplementedException();
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
