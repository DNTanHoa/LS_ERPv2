namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class IncoTerm
    {
        public IncoTerm()
        {
            Code = string.Empty;
        }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }
    }
}