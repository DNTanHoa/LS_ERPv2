namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Company : Audit
    {
        public Company()
        {
            Code = string.Empty;
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string OrtherName { get; set; }
        public string ShortName { get; set; }
        public string DisplayName { get; set; }
        public string Phone { get; set; }
        public string DisplayPhone { get; set; }
        public string Email { get; set; }
        public string DisplayEmail { get; set; }
        public string FaxNumber { get; set; }
        public string DisplayFaxNumber { get; set; }
        public string Address { get; set; }
        public string DisplayAddress { get; set; }
        public long? BankAccountID { get; set; }
        public virtual BankAccount BankAccount { get; set; }
    }
}