using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class MaterialSyncSearchParamAction_ViewController : ViewController
    {
        public MaterialSyncSearchParamAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchItemStyleInMaterialSyncAction = new SimpleAction(this, 
                "SearchItemStyleInMaterialSyncMaster", PredefinedCategory.Unspecified);
            searchItemStyleInMaterialSyncAction.ImageName = "Action_Search_Object_FindObjectByID";
            searchItemStyleInMaterialSyncAction.Caption = "Search (Ctrl + L)";
            searchItemStyleInMaterialSyncAction.TargetObjectType = typeof(MaterialSyncMasterSearchParam);
            searchItemStyleInMaterialSyncAction.TargetViewType = ViewType.DetailView;
            searchItemStyleInMaterialSyncAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchItemStyleInMaterialSyncAction.Execute += SearchMaterialSyncAction_Execute;

            SimpleAction loadMaterialSyncAction = new SimpleAction(this,
                "LoadMaterialSyncMaster", PredefinedCategory.Unspecified);
            loadMaterialSyncAction.ImageName = "ResetLayoutOptions";
            loadMaterialSyncAction.Caption = "Load Material (Ctrl + M)";
            loadMaterialSyncAction.TargetObjectType = typeof(MaterialSyncMasterSearchParam);
            loadMaterialSyncAction.TargetViewType = ViewType.DetailView;
            loadMaterialSyncAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            loadMaterialSyncAction.Execute += LoadMaterialSyncAction_Execute;

            PopupWindowShowAction importMaterialSync = new PopupWindowShowAction(this, "ImportMaterialSync", PredefinedCategory.Unspecified);
            importMaterialSync.ImageName = "Import";
            importMaterialSync.Caption = "Import (Ctrl + I)";
            importMaterialSync.TargetObjectType = typeof(MaterialSyncMasterSearchParam);
            importMaterialSync.TargetViewType = ViewType.Any;
            importMaterialSync.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            importMaterialSync.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            importMaterialSync.Shortcut = "CtrlI";

            importMaterialSync.CustomizePopupWindowParams += ImportMaterialSync_CustomizePopupWindowParams;
            importMaterialSync.Execute += ImportMaterialSync_Execute;

        }

        private void ImportMaterialSync_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {

        }

        private void ImportMaterialSync_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {

        }

        private void LoadMaterialSyncAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            
        }

        private void SearchMaterialSyncAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as MaterialSyncMasterSearchParam;
            MessageOptions message = null;

            if (viewObject != null)
            {
                var criteria = CriteriaOperator.Parse("(Contains(LSStyle,?) OR ?) AND (CustomerID = ? OR ?)",
                    viewObject.Style, string.IsNullOrEmpty(viewObject.Style),
                    viewObject.Customer?.ID, string.IsNullOrEmpty(viewObject.Customer?.ID));

                var itemStyleSyncMasters = ObjectSpace.GetObjects<ItemStyleSyncMaster>(criteria);
                viewObject.ItemStyleSyncMasters = itemStyleSyncMasters.ToList();
                View.Refresh();
            }
            else
            {
                message = Message.GetMessageOptions("Unknown error, contact your admin", "Error",
                    InformationType.Error, null, 5000);

            }

            if (message != null)
                Application.ShowViewStrategy.ShowMessage(message);
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
