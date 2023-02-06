namespace Ultils.Config
{
    public class StaticFileConfig
    {
        public static string ConfigName => "StaticFile";
        public string DefaultDirectory { get; set; } = string.Empty;
        public string ImportDirectory { get; set; } = string.Empty;
    }
}