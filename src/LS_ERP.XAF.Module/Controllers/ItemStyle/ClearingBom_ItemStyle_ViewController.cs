using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using System;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ClearingBom_ItemStyle_ViewController : ViewController
    {
        public ClearingBom_ItemStyle_ViewController()
        {
            InitializeComponent();
            SimpleAction clearingBomAction = new SimpleAction(this, "ClearingBomItemStyle", PredefinedCategory.Unspecified);
            clearingBomAction.ImageName = "CalcDefault";
            clearingBomAction.Caption = "Clearing BOM";
            clearingBomAction.TargetObjectType = typeof(ItemStyle);
            clearingBomAction.TargetViewType = ViewType.ListView;
            clearingBomAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            clearingBomAction.Execute += ClearingBomAction_Execute; ;
        }

        private void ClearingBomAction_Execute(object sender, SimpleActionExecuteEventArgs e)
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
