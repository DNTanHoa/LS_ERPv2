using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.XtraPivotGrid;
using LS_ERP.XAF.Module.DomainComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LS_ERP.XAF.Module.Win.Controllers.View
{
    public partial class PackingRatio_PivotGridEditable_ViewController : ViewController
    {
        public PackingRatio_PivotGridEditable_ViewController()
        {
            InitializeComponent();
            TargetObjectType = typeof(PackingRatio);
            TargetViewType = ViewType.ListView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            PivotGridListEditor pivotGridEditor = ((ListView)View).Editor as PivotGridListEditor;
            if (pivotGridEditor != null)
            {
                PivotGridControl pivotGridControl = pivotGridEditor.PivotGridControl;
            }
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
