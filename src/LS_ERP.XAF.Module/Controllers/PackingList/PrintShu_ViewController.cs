using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using DevExpress.XtraReports.UI;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.Report;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class PrintShu_ViewController 
        : ObjectViewController<ListView, PackingList>
    {
        public PrintShu_ViewController()
        {
            InitializeComponent();

            SimpleAction printShu = new SimpleAction(this, "PrintShu", PredefinedCategory.Unspecified);
            printShu.ImageName = "ProductQuickShippments";
            printShu.Caption = "Print Shu";
            printShu.TargetObjectType = typeof(PackingList);
            printShu.TargetViewType = ViewType.ListView;
            printShu.PaintStyle = ActionItemPaintStyle.CaptionAndImage;
            printShu.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
            printShu.Shortcut = "CtrlM";

            printShu.Execute += PrintShu_Execute;
        }

        private void PrintShu_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var packingList = View.CurrentObject as PackingList;

            if (packingList != null)
            {
                var dataSource = new List<ShuModel>();
                var connectionString = Application.ConnectionString;

                using(var db = new QueryFactory(
                   new SqlConnection(connectionString), new SqlServerCompiler()))
                {
                    dataSource = db.Select<ShuModel>
                        ("EXEC sp_SelectPackingListForPrintShu @PackingListID",
                        new { PackingListID = packingList.ID }).ToList();
                }

                var shu = new ShuTemp();
                shu.DataSource = dataSource;
                ReportPrintTool ShuPrintTool = new ReportPrintTool(shu);
                ShuPrintTool.ShowPreview();
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
