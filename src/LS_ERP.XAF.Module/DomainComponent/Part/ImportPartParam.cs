using DevExpress.ExpressApp.DC;
using LS_ERP.EntityFrameworkCore.Entities;

namespace LS_ERP.XAF.Module.DomainComponent
{
    [DomainComponent]
    public class ImportPartParam
    {
        public ImportPartParam()
        {
            this.SalesOrderIDs = "SO.HADDAD.2022;SO.HADDAD.2023";
        }
        public Customer Customer { get; set; }
        public string SalesOrderIDs { get; set; }
        public string FilePath { get; set; }
        public bool Update { get; set; }
    }
}
