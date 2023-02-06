namespace Logic.Config;
public class AuthenticateConfig
{
    public static string ConfigName { get; set; } = "Authenticate";
    public string Issuer { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int TokenExpireAfterMinutes { get; set; } = 0;
    public string DefaultRedirect { get; set; } = string.Empty;
}
