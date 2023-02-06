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
using LS_ERP.XAF.Module.Service.Request;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers.DomainComponent
{
    public partial class JobOperationSearchParamAction_ViewController : ViewController
    {
        public JobOperationSearchParamAction_ViewController()
        {
            InitializeComponent();

            SimpleAction loadJobHeadToOperation = new SimpleAction(this, "LoadJobHeadToOperation", PredefinedCategory.Unspecified);
            loadJobHeadToOperation.ImageName = "GridResetColumnWidths";
            loadJobHeadToOperation.Caption = "Load Job (Ctrl + L)";
            loadJobHeadToOperation.TargetObjectType = typeof(JobOperationSearchParam);
            loadJobHeadToOperation.TargetViewType = ViewType.DetailView;
            loadJobHeadToOperation.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            loadJobHeadToOperation.Shortcut = "CtrlL";

            loadJobHeadToOperation.Execute += LoadJobHeadToOperation_Execute;

            SimpleAction copyJobHeadToOperation = new SimpleAction(this, "CopyJobHeadToOperation", PredefinedCategory.Unspecified);
            copyJobHeadToOperation.ImageName = "BO_Transition";
            copyJobHeadToOperation.Caption = "Entry (Ctrl + E)";
            copyJobHeadToOperation.TargetObjectType = typeof(JobOperationSearchParam);
            copyJobHeadToOperation.TargetViewType = ViewType.DetailView;
            copyJobHeadToOperation.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            copyJobHeadToOperation.Shortcut = "CtrlE";

            copyJobHeadToOperation.Execute += CopyJobHeadToOperation_Execute;

            SimpleAction submitJobOperation = new SimpleAction(this, "SubmitJobOperation", PredefinedCategory.Unspecified);
            submitJobOperation.ImageName = "Actions_Send";
            submitJobOperation.Caption = "Submit (Ctrl + I)";
            submitJobOperation.TargetObjectType = typeof(JobOperationSearchParam);
            submitJobOperation.TargetViewType = ViewType.DetailView;
            submitJobOperation.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            submitJobOperation.Shortcut = "CtrlI";

            submitJobOperation.Execute += SubmitJobOperation_Execute;
        }

        private void SubmitJobOperation_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as JobOperationSearchParam;
            MessageOptions message = null;

            if(viewObject != null)
            {
                var configuration = new MapperConfiguration(c => 
                {
                    c.CreateMap<JobOperation, JobOperationDto>();
                });
                var mapper = configuration.CreateMapper();

                var service = new JobOperationService();
                var request = new BulkJobOperationRequest()
                {
                    Username = SecuritySystem.CurrentUserName,
                    Data = viewObject.JobOperations
                        .Select(x => mapper.Map<JobOperationDto>(x)).ToList()
                };

                var response = service.BulkJobOperation(request).Result;

                if(response != null)
                {
                    if(response.Success)
                    {
                        viewObject.JobOperations = new List<JobOperation>();
                        message = Message.GetMessageOptions("Action successfully",
                            "Success", InformationType.Success, null, 5000);
                    }
                    else
                    {
                        message = Message.GetMessageOptions(response.Message,
                            "Error", InformationType.Error, null, 5000);
                    }
                }
                else
                {
                    message = Message.GetMessageOptions("Unexpected error. Contact your admin",
                        "Error", InformationType.Error, null, 5000);
                }

                if (message != null)
                    Application.ShowViewStrategy.ShowMessage(message);

                View.Refresh();
            }
        }

        private void CopyJobHeadToOperation_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as JobOperationSearchParam;

            if(viewObject != null)
            {
                ListPropertyEditor listPropertyEditor = 
                    ((DetailView)View).FindItem("JobHeads") as ListPropertyEditor;

                if(listPropertyEditor != null)
                {
                    var selectedStyles = listPropertyEditor.ListView
                        .SelectedObjects.Cast<JobHead>().ToList();

                    var jobOperations = selectedStyles.Select(x => new JobOperation()
                    {
                        ID = Nanoid.Nanoid.Generate("ABCDEFGHIJKLMNOPQRST123456789", 15),
                        JobHeadNumber = x.Number,
                        RequiredQuantity = x.ProductionQuantity + (x.SampleQuantity ?? 0)
                            + ((x.OverPercent ?? 0) * x.ProductionQuantity / 100),
                        CompletedQuantity = 0,
                        RemainQuantity = x.ProductionQuantity + (x.SampleQuantity ?? 0)
                            + ((x.OverPercent ?? 0) * x.ProductionQuantity / 100),
                        EstimateStartDate = DateTime.Now,
                        EstimateEndDate = DateTime.Now.AddDays(3),
                        StartDate = DateTime.Now,
                        Name = viewObject.Operation.Name,
                        OtherName = viewObject.Operation.Name,
                        Index = viewObject.Operation.Index,
                        JobHead = x,
                        CustomerID = viewObject.Customer?.ID
                    });

                    viewObject.JobOperations = jobOperations.ToList();
                }

                View.Refresh();
            }
        }

        private void LoadJobHeadToOperation_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as JobOperationSearchParam;
            
            if(viewObject != null)
            {
                var service = new JobHeadService();
                var request = new JobHeadFilterRequest()
                {
                    CustomerID = viewObject.Customer?.ID,
                    Style = viewObject.Style,
                    FromDate = viewObject.FromDate,
                    ToDate = viewObject.ToDate,
                };
                var response = service.GetJobHeadFilter(request).Result;

                if (response.IsSuccess)
                {
                    viewObject.JobHeads = response.Data.ToList();
                    View.Refresh();
                }
                else
                {
                    var message = Message.GetMessageOptions(response.ErrorMessage, "Error",
                        InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(message);
                }
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
