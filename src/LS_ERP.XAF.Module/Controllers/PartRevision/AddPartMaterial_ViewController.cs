using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class AddPartMaterial_ViewController : ViewController
    {
        public AddPartMaterial_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction addPartMaterialToPartRevision = 
                new PopupWindowShowAction(this, "AddPartMaterialToPartRevision", PredefinedCategory.Unspecified);
            addPartMaterialToPartRevision.ImageName = "Import";
            addPartMaterialToPartRevision.Caption = "Import (Ctrl + I)";
            addPartMaterialToPartRevision.TargetObjectType = typeof(PartRevision);
            addPartMaterialToPartRevision.TargetObjectsCriteria = "ID > 0";
            addPartMaterialToPartRevision.TargetViewType = ViewType.DetailView;
            addPartMaterialToPartRevision.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            addPartMaterialToPartRevision.Shortcut = "CtrlI";

            addPartMaterialToPartRevision.Execute += AddPartMaterialToPartRevision_Execute;
            addPartMaterialToPartRevision.CustomizePopupWindowParams += AddPartMaterialToPartRevision_CustomizePopupWindowParams;
        }

        private void AddPartMaterialToPartRevision_CustomizePopupWindowParams(object sender, 
            CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = this.ObjectSpace;
            var partRevison = View.CurrentObject as PartRevision;
            var model = new ImportPartRevisionParam()
            {
                ID = partRevison.ID,
                StyleNumber = partRevison.PartNumber,
                RevisionNumber = partRevison.RevisionNumber,
                EffectDate = partRevison.EffectDate,
                IsConfirmed = partRevison.IsConfirmed,
                Customer = partRevison.Customer,
                Season = partRevison.Season,
            };

            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.View = view;
        }

        private void AddPartMaterialToPartRevision_Execute(object sender, 
            PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowViewCurrentObject as ImportPartRevisionParam;
            var partRevision = View.CurrentObject as PartRevision;
            var userName = SecuritySystem.CurrentUserName;

            if (partRevision != null &&
               importParam.PartMaterials != null &&
               importParam.PartMaterials.Any())
            {
                foreach (var partMaterial in importParam.PartMaterials)
                {
                    var existPartMaterial = partRevision.PartMaterials
                        .FirstOrDefault(x => x.ExternalCode == partMaterial.ExternalCode &&
                                             x.GarmentColorCode == partMaterial.GarmentColorCode &&
                                             x.ItemStyleNumber == partMaterial.ItemStyleNumber);

                    if(existPartMaterial != null)
                    {
                        var message = Message.GetMessageOptions("Exist item with external code " + existPartMaterial.ExternalCode +
                            " for garment color " + existPartMaterial.GarmentColorCode + " with style " + existPartMaterial.ItemStyleNumber,
                            "Error", InformationType.Error, null, 5000);
                        Application.ShowViewStrategy.ShowMessage(message);
                        return;
                    }
                }

                foreach (var partmaterial in importParam.PartMaterials)
                {
                    partRevision.PartMaterials.Add(partmaterial);
                }

                if(string.IsNullOrEmpty(partRevision.CreatedBy))
                {
                    partRevision.SetCreateAudit(userName);
                }
                else
                {
                    partRevision.SetUpdateAudit(userName);
                }

                ObjectSpace.CommitChanges();
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
