namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class SupplierCNUF : Audit
    {
        public SupplierCNUF()
        {
            Code = string.Empty;
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string SiteName { get; set; }
        public string DPPOfice { get; set; }
        public string SiteCountry { get; set; }
        public string ProcessLeader { get; set; }
        public string CustomerID { get; set; }
        public string FileName { get; set; }

        public virtual Customer Customer { get; set; }
    }
}