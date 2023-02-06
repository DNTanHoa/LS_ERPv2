using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class ImportForecast_ViewController : ViewController
    {
        public ImportForecast_ViewController()
        {
            InitializeComponent();

            PopupWindowShowAction importForecast = new PopupWindowShowAction(this, "ImportForecast", PredefinedCategory.Unspecified);
            importForecast.ImageName = "Import";
            importForecast.Caption = "Import (Ctrl + I)";
            importForecast.TargetObjectType = typeof(ForecastGroup);
            importForecast.TargetViewType = ViewType.Any;
            importForecast.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            importForecast.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            importForecast.Shortcut = "CtrlI";

            importForecast.CustomizePopupWindowParams += ImportForecast_CustomizePopupWindowParams;
            importForecast.Execute += ImportForecast_Execute;
        }

        private void ImportForecast_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var importParam = e.PopupWindowViewCurrentObject as ForecastGroupImportParam;
            var forecastGroup = View.CurrentObject as ForecastGroup;

            forecastGroup.ForecastEntries?.ToList().ForEach(e =>
            {
                e.IsDeActive = true;
            });

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            FileInfo fileInfo = new FileInfo(importParam.FilePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet workSheet = null;
                if(package.Workbook.Worksheets.Count() > 2)
                {
                    workSheet = package.Workbook.Worksheets[1];
                }
                var forcastEntry = ObjectSpace.CreateObject<ForecastEntry>();

                forcastEntry.ForecastGroup = forecastGroup;
                forcastEntry.ForecastOveralls = importParam.Overalls;
                forcastEntry.EntryDate = DateTime.Now;
                forcastEntry.Name = Path.GetFileNameWithoutExtension(importParam.FilePath);
                forcastEntry.Title = workSheet?.Name ?? forcastEntry.Name;

                forcastEntry.SetCreateAudit(SecuritySystem.CurrentUserName);
            }

            ObjectSpace.CommitChanges();
            View.Refresh();
        }

        private void ImportForecast_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var forecastGroup = View.CurrentObject as ForecastGroup;
            var objectSpace = this.ObjectSpace;
            var model = new ForecastGroupImportParam() 
            {
                Customer = forecastGroup.Customer,
                ForecastGroup = forecastGroup
            };
            var view = Application.CreateDetailView(objectSpace, model, false);
            e.DialogController.SaveOnAccept = false;
            e.Maximized = true;
            e.View = view;
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
