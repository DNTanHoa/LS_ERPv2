using AutoMapper;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Dtos;
using LS_ERP.XAF.Module.Helpers;
using LS_ERP.XAF.Module.Service;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class IssuedProductionBom_ViewController : ViewController
    {
        public IssuedProductionBom_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction issuedProductionBomAction = new PopupWindowShowAction(this, "IssuedProductionBomAction",
                PredefinedCategory.Unspecified);

            issuedProductionBomAction.ImageName = "Header";
            issuedProductionBomAction.Caption = "Production Bom";
            issuedProductionBomAction.TargetObjectType = typeof(Issued);
            issuedProductionBomAction.TargetViewType = ViewType.DetailView;
            issuedProductionBomAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            issuedProductionBomAction.Shortcut = "CtrlShiftB";

            issuedProductionBomAction.CustomizePopupWindowParams += 
                IssuedProductionBomAction_CustomizePopupWindowParams;
            issuedProductionBomAction.Execute += IssuedProductionBomAction_Execute;
        }

        private void IssuedProductionBomAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var issued = View.CurrentObject as Issued;

            ListPropertyEditor listPropertyEditor = ((DetailView)e.PopupWindowView)
                .FindItem("ProductionBOMs") as ListPropertyEditor;
            MessageOptions message = null;

            var config = new MapperConfiguration(c =>
            {
                c.CreateMap<ProductionBOM, ProductionBOMDto>()
                    .ForMember(x => x.GarmentColorName, y => y.MapFrom(s => s.ItemStyle.ColorName))
                    .ForMember(x => x.GarmentColorCode, y => y.MapFrom(s => s.ItemStyle.ColorCode));

                c.CreateMap<ItemStyle, ItemStyleDto>();
                c.CreateMap<PurchaseOrderLine, IssuedLineDto>();
                c.CreateMap<PurchaseOrderGroupLine, IssuedGroupLineDto>();
            });

            var mapper = config.CreateMapper();

            var service = new ProductionBOMService();

            var request = new GroupToIssuedLineRequest()
            {
                IssuedLines = issued.IssuedLines?
                                        .Select(x => mapper.Map<IssuedLineDto>(x)).ToList(),
                IssuedGroupLines = issued.IssuedGroupLines?
                                        .Select(x => mapper.Map<IssuedGroupLineDto>(x)).ToList(),
                ProductionBOMs = listPropertyEditor.ListView.SelectedObjects.Cast<ProductionBOM>()?
                                        .Select(x => mapper.Map<ProductionBOMDto>(x)).ToList()
            };

            var response = service.GroupToIssuedLine(request).Result;

            if (response != null)
            {
                if (response.Result.Code == "000")
                {
                    List<IssuedGroupLine> groupLines = null;
                    List<IssuedLine> lines = null;
                    if (issued.IssuedGroupLines == null)
                    {
                        groupLines = new List<IssuedGroupLine>();
                    }
                    else
                    {
                        groupLines = issued.IssuedGroupLines.ToList();
                    }

                    if (issued.IssuedLines == null)
                    {
                        lines = new List<IssuedLine>();
                    }
                    else
                    {
                        lines = issued.IssuedLines.ToList();
                    }

                    groupLines.AddRange(response.Data?.IssuedGroupLines);
                    lines.AddRange(response.Data?.IssuedLines);

                    issued.IssuedLines = lines;
                    issued.IssuedGroupLines = groupLines;
                }
                else
                {
                    message = Message.GetMessageOptions(response.Result.Message, "Error", InformationType.Error,
                        null, 5000);
                }
            }
            else
            {
                message = Message.GetMessageOptions("Unknown error. Contact your admin", "Error", InformationType.Error,
                        null, 5000);
            }

            if (message != null)
                Application.ShowViewStrategy.ShowMessage(message);

            View.Refresh();
        }

        private void IssuedProductionBomAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var issued = View.CurrentObject as Issued;
            var objectSpace = this.ObjectSpace;
            var model = new IssuedCreateParam()
            {
                CustomerID = issued.CustomerID,
                StorageCode = issued.StorageCode,
            };
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = Application.CreateDetailView(objectSpace, model, false);
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
