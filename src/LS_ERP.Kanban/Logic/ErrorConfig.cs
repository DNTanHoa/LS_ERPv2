namespace Logic.Config;
public class ErrorConfig
{
    public static string ConfigName { get; set; } = "Error";
    public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();

    public string GetByKey(string key)
    {
        var error = Errors.FirstOrDefault(x => x.Key == key);
        return error != null ? error.Value : string.Empty;
    }
}

public class ErrorModel
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}