using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using LS_ERP.XAF.Module.Service.Request;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class DeleteProductionBom_ViewController : ObjectViewController<ListView, ProductionBOM>
    {
        public DeleteProductionBom_ViewController()
        {
            InitializeComponent();

            SimpleAction deleteProductionBOM = new SimpleAction(this, "DeleteProductionBOM", PredefinedCategory.Unspecified);
            deleteProductionBOM.ImageName = "Delete";
            deleteProductionBOM.Caption = "Delete (Ctrl + D)";
            deleteProductionBOM.TargetObjectType = typeof(ProductionBOM);
            deleteProductionBOM.TargetViewId = "ViewProductionBomParam_ProductionBOMs_ListView";
            deleteProductionBOM.TargetObjectsCriteria = "Not(ReservedQuantity > 0)";
            deleteProductionBOM.TargetViewNesting = Nesting.Nested;
            deleteProductionBOM.TargetViewType = ViewType.ListView;
            deleteProductionBOM.PaintStyle = ActionItemPaintStyle.Image;

            deleteProductionBOM.Execute += DeleteProductionBOM_Execute;
        }

        private void DeleteProductionBOM_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var config = new MapperConfiguration(c =>
            {
                c.CreateMap<ProductionBOM, LS_ERP.XAF.Module.Dtos.ProductionBOMDto>()
                    .ForMember(x => x.ItemStyle, y => y.Ignore());
            });
            var mapper = config.CreateMapper();

            var request = new DeleteProductionBOMRequest()
            {
                ProductionBOMs = View.SelectedObjects.Cast<ProductionBOM>()
                                    .Select(x => mapper.Map<LS_ERP.XAF.Module.Dtos.ProductionBOMDto>(x)).ToList()
            };

            var service = new ProductionBOMService();
            MessageOptions message = null;
            var response = service.Delete(request).Result;

            if(response != null)
            {
                if(response.Result.Code == "000")
                {
                    message = Message.GetMessageOptions("Delete success fully", "Success",
                        InformationType.Success, null, 5000);
                }
                else
                {
                    message = Message.GetMessageOptions(response.Result.Message, "Error",
                        InformationType.Error, null, 5000);
                }
            }

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
