using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using System.Collections.Generic;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class UpRevision_ViewController : ViewController
    {
        public UpRevision_ViewController()
        {
            InitializeComponent();

            SimpleAction upPartRevision = new SimpleAction(this, "UpRevision", PredefinedCategory.Unspecified);
            upPartRevision.ImageName = "Update";
            upPartRevision.Caption = "Up Version (Ctrl + U)";
            upPartRevision.TargetObjectType = typeof(PartRevision);
            upPartRevision.TargetObjectsCriteria = "ID > 0";
            upPartRevision.TargetViewType = ViewType.Any;
            upPartRevision.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            upPartRevision.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            upPartRevision.Shortcut = "CtrlU";

            upPartRevision.Execute += UpPartRevision_Execute;
        }

        private void UpPartRevision_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var currentPartRevision = View.CurrentObject as PartRevision;
            var objectSpace = Application.CreateObjectSpace(typeof(PartRevision));

            if(currentPartRevision != null)
            {
                var newPartRevision = objectSpace.CreateObject<PartRevision>();
                
                /// Map part revision
                newPartRevision.PartNumber = currentPartRevision.PartNumber;
                newPartRevision.RevisionNumber = currentPartRevision.RevisionNumber;
                newPartRevision.Season = currentPartRevision.Season;
                newPartRevision.EffectDate = currentPartRevision.EffectDate;
                newPartRevision.IsConfirmed = false;
                newPartRevision.CustomerID = currentPartRevision.CustomerID;
                newPartRevision.PartMaterials = new List<PartMaterial>();

                /// Map part material
                foreach(var partMaterial in currentPartRevision.PartMaterials)
                {
                    var newPartMaterial = objectSpace.CreateObject<PartMaterial>();

                    newPartMaterial.ItemCode = partMaterial.ItemCode;
                    newPartMaterial.ItemColorCode = partMaterial.ItemColorCode;
                    newPartMaterial.ItemColorName = partMaterial.ItemColorName;
                    newPartMaterial.ItemID = partMaterial.ItemID;
                    newPartMaterial.ItemName = partMaterial.ItemName;
                    newPartMaterial.ExternalCode = partMaterial.ExternalCode;
                    newPartMaterial.Position = partMaterial.Position;
                    newPartMaterial.LabelCode = partMaterial.LabelCode;
                    newPartMaterial.LabelName = partMaterial.LabelName;
                    newPartMaterial.MaterialTypeCode = partMaterial.MaterialTypeCode;
                    newPartMaterial.MaterialTypeClass = partMaterial.MaterialTypeClass;
                    newPartMaterial.Specify = partMaterial.Specify;
                    newPartMaterial.Division = partMaterial.Division;
                    newPartMaterial.GarmentSize = partMaterial.GarmentSize;
                    newPartMaterial.GarmentColorCode = partMaterial.GarmentColorCode;
                    newPartMaterial.GarmentColorName = partMaterial.GarmentColorName;
                    newPartMaterial.VendorID = partMaterial.VendorID;
                    newPartMaterial.PerUnitID = partMaterial.PerUnitID;
                    newPartMaterial.PriceUnitID = partMaterial.PriceUnitID;
                    newPartMaterial.CurrencyID = partMaterial.CurrencyID;
                    newPartMaterial.Price = partMaterial.Price;
                    newPartMaterial.QuantityPerUnit = partMaterial.QuantityPerUnit;
                    newPartMaterial.LeadTime = partMaterial.LeadTime;
                    newPartMaterial.LessPercent = partMaterial.LessPercent;
                    newPartMaterial.FreePercent = partMaterial.FreePercent;
                    newPartMaterial.WastagePercent = partMaterial.WastagePercent;
                    newPartMaterial.OverPercent = partMaterial.OverPercent;
                    newPartMaterial.FabricWeight = partMaterial.FabricWeight;
                    newPartMaterial.FabricWidth = partMaterial.FabricWidth;
                    newPartMaterial.CutWidth = partMaterial.CutWidth;

                    newPartMaterial.PartMaterialStatus = null;
                    newPartMaterial.PartMaterialStatusCode = "0";
                    newPartMaterial.PartRevision = null;
                    newPartMaterial.PartRevisionID = null;

                    newPartRevision.PartMaterials.Add(newPartMaterial);
                }
                     
                e.ShowViewParameters.CreatedView =
                    Application.CreateDetailView(objectSpace, newPartRevision, true);
            }
            else
            {
                var message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error",
                    InformationType.Error, null, 5000);
                Application.ShowViewStrategy.ShowMessage(message);
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
