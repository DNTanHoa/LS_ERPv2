using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class MaterialRequestSearchParam_ViewController : ViewController
    {
        public MaterialRequestSearchParam_ViewController()
        {
            InitializeComponent();

            SimpleAction materialRequestSearchAction = new SimpleAction(this, "MaterialRequestSearchAction", PredefinedCategory.Unspecified);
            materialRequestSearchAction.ImageName = "Action_Search_Object_FindObjectByID";
            materialRequestSearchAction.Caption = "Search (Shift + P)";
            materialRequestSearchAction.TargetObjectType = typeof(MaterialRequestSearchParam);
            materialRequestSearchAction.TargetViewType = ViewType.DetailView;
            materialRequestSearchAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            materialRequestSearchAction.Execute += MaterialRequestSearchAction_Execute;
        }

        private void MaterialRequestSearchAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as MaterialRequestSearchParam;

            if(viewObject != null)
            {
                var criteria = CriteriaOperator.Parse("([StorageCode] = ? OR ?) AND [RequestDate] >= ? AND [RequestDate] <= ?",
                    viewObject.StorageCode?.Code, string.IsNullOrEmpty(viewObject.StorageCode?.Code), 
                    viewObject.FromDate, viewObject.ToDate);
                var materialRequest = ObjectSpace.GetObjects<MaterialRequest>(criteria);
                viewObject.MaterialRequests = materialRequest.ToList(); 
                View.Refresh();
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
