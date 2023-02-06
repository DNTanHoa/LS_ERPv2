using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.XAF.Module.DomainComponent;
using LS_ERP.XAF.Module.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Ultils.Helpers;

namespace LS_ERP.XAF.Module.Controllers
{
    public partial class QualityAssuranceSearchParamAction_ViewController : ObjectViewController<DetailView, QualityAssuranceSearchParam>
    {
       
        public QualityAssuranceSearchParamAction_ViewController()
        {
            InitializeComponent();

            SimpleAction searchQualityAssurance = new SimpleAction(this, "SearchQualityAssurance", PredefinedCategory.Unspecified);
            searchQualityAssurance.ImageName = "Action_Search_Object_FindObjectByID";
            searchQualityAssurance.Caption = "Search (Ctrl + L)";
            searchQualityAssurance.TargetObjectType = typeof(QualityAssuranceSearchParam);
            searchQualityAssurance.TargetViewType = ViewType.DetailView;
            searchQualityAssurance.PaintStyle = ActionItemPaintStyle.CaptionAndImage;

            searchQualityAssurance.Execute += SearchQualityAssurance_Execute;
        }
        private void SearchQualityAssurance_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var viewObject = View.CurrentObject as QualityAssuranceSearchParam;
            var errorMessage = "";

            if (viewObject != null)
            {
                var qualityAssurances = new List<QualityAssurance>();
                try 
                {
                    var qualityStatus = ObjectSpace.GetObjects<QualityStatus>();
                    var connectionString = Application.ConnectionString ?? string.Empty;
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@CustomerID",viewObject?.Customer?.ID ?? string.Empty),
                        new SqlParameter("@StatusID",viewObject?.Status?.ID ?? string.Empty),
                        new SqlParameter("@Percent",viewObject?.Percent ?? 0)
                    };

                    DataTable table = SqlHelper.FillByReader_ItemMasterJob(connectionString, "sp_LoadQualityAssurance", parameters);
                   foreach(DataRow dr in table.Rows)
                    {
                        var qualityAssurance = new QualityAssurance()
                        {
                            ID = (long)dr["ID"],
                            ItemStyleNumber = dr["ItemStyleNumber"].ToString(),
                            PurchaseOrderNumber = dr["PurchaseOrderNumber"].ToString(),
                            CustomerID = dr["CustomerID"].ToString(),
                            StorageCode = dr["StorageCode"].ToString(),
                            ApprovedDate = (DateTime)dr["ApprovedDate"],
                            LSStyle = dr["LSStyle"].ToString(),
                            CustomerStyle = dr["CustomerStyle"].ToString(),
                            Season = dr["Season"].ToString(),
                            GarmentColorCode = dr["GarmentColorCode"].ToString(),
                            GarmentColorName = dr["GarmentColorName"].ToString(),
                            GarmentSize = dr["GarmentSize"].ToString(),
                            BinCode = dr["BinCode"].ToString(),
                            OrderQuantity = (decimal)dr["OrderQuantity"],
                            Quantity = (decimal)dr["Quantity"],
                            Percent = (decimal)dr["Percent"] >= 100 ? (decimal)100.00 : Math.Round((decimal)dr["Percent"], 2),
                            QualityStatusID = dr["QualityStatusID"].ToString(),
                            Remark = dr["Remark"].ToString(),
                        };
                        qualityAssurance.SetCreateAudit(SecuritySystem.CurrentUserName);
                        qualityAssurance.QualityStatus = qualityStatus.FirstOrDefault(x => x.ID == qualityAssurance.QualityStatusID);

                        qualityAssurances.Add(qualityAssurance);

                    }
                }
                catch (Exception ex)
                {
                    var error = Message.GetMessageOptions(errorMessage, "Error", InformationType.Error, null, 5000);
                    Application.ShowViewStrategy.ShowMessage(error);
                }

                viewObject.QualityAssurances = qualityAssurances;
            }

            View.Refresh();
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
