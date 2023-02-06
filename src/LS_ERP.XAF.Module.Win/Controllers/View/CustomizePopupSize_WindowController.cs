using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LS_ERP.XAF.Module.Win.Controllers.View
{
    public partial class CustomizePopupSize_WindowController : WindowController
    {
        public CustomizePopupSize_WindowController()
        {
            InitializeComponent();
            this.TargetWindowType = WindowType.Main;
        }
        
        protected override void OnActivated()
        {
            base.OnActivated();

        }
        
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
    }
}
