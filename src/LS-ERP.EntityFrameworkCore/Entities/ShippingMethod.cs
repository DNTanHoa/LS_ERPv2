namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class ShippingMethod : Audit
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; }
        public int? LeadTime { get; set; }
        public decimal? Price { get; set; }
    }
}