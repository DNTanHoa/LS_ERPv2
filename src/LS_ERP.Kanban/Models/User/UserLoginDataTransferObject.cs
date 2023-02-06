namespace LS_ERP.Kanban.Models
{
    public class UserLoginDataTransferObject
    {
        public string UserName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiredDate { get; set; }
        public bool isAdmin { get; set; }
    }
}