namespace LS_ERP.Kanban.Models
{

    public class LoginViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; } = string.Empty;
        public string? RedirectUrl { get; set; } = string.Empty;
        public string? ErrorUrl { get; set; } = string.Empty;
        public bool RemmemberMe { get; set; }
    }
}
