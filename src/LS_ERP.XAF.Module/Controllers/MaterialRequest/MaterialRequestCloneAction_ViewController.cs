using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class MaterialRequestCloneAction_ViewController : ObjectViewController<ListView, MaterialRequest>
    {
        public MaterialRequestCloneAction_ViewController()
        {
            InitializeComponent();

            SimpleAction materialRequestCloneAction = new SimpleAction(this, "MaterialRequestCloneAction", PredefinedCategory.Unspecified);
            materialRequestCloneAction.ImageName = "Copy";
            materialRequestCloneAction.Caption = "Clone (Shift + C)";
            materialRequestCloneAction.TargetObjectType = typeof(MaterialRequest);
            materialRequestCloneAction.TargetViewType = ViewType.ListView;
            materialRequestCloneAction.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            materialRequestCloneAction.Shortcut = "ShiftC";

            materialRequestCloneAction.Execute += MaterialRequestCloneAction_Execute;
        }

        private void MaterialRequestCloneAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace(typeof(MaterialRequest));
            var materialRequests = ((ListView)View).CollectionSource.List;

            var viewObject = View.CurrentObject as MaterialRequest;

            if (viewObject != null)
            {
                var mappingConfig = new MapperConfiguration(c =>
                {
                    c.CreateMap<MaterialRequestDetail, MaterialRequestDetail>()
                        .ForMember(x => x.Id, y => y.Ignore())
                        .ForMember(x => x.MaterialRequestId, y => y.Ignore())
                        .ForMember(x => x.Quantity, y => y.MapFrom(q => 0));
                    c.CreateMap<MaterialRequestOrderDetail, MaterialRequestOrderDetail>()
                        .ForMember(x => x.ID, y => y.Ignore())
                        .ForMember(x => x.MaterialRequestId, y => y.Ignore());
                });

                var mapper = mappingConfig.CreateMapper();

                try
                {
                    var cloneRequest = objectSpace.CreateObject<MaterialRequest>();
                    cloneRequest.RequestDate = DateTime.Now;
                    cloneRequest.DueDate = DateTime.Now;
                    cloneRequest.RequestFor = viewObject.RequestFor;
                    cloneRequest.Remark = viewObject.Remark;
                    cloneRequest.IsProcess = viewObject.IsProcess;
                    cloneRequest.IsSuccess = viewObject.IsSuccess;
                    cloneRequest.ErrorMessage = viewObject.ErrorMessage;
                    cloneRequest.StorageCode = viewObject.StorageCode;
                    cloneRequest.CustomerID = viewObject.CustomerID;
                    cloneRequest.LSStyles = viewObject.LSStyles;
                    cloneRequest.CloneRequestID = viewObject.Id;
                    cloneRequest.SetCreateAudit(SecuritySystem.CurrentUserName);

                    cloneRequest.Details = viewObject?.Details
                                .Select(x => mapper.Map<MaterialRequestDetail>(x)).ToList();

                    cloneRequest.OrderDetails = viewObject?.OrderDetails
                               .Select(x => mapper.Map<MaterialRequestOrderDetail>(x)).ToList();

                    objectSpace.CommitChanges();

                    materialRequests.Add(cloneRequest);

                    var error = Message.GetMessageOptions("Clone Successful", "Success", InformationType.Success, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(error);

                }
                catch (Exception ex)
                {
                    var error = Message.GetMessageOptions(ex.Message, "Error", InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(error);
                }

                View.Refresh();
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
