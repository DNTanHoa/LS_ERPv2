namespace LS_ERP.API.Helpers
{
    public static class UrlHelpers
    {
        public static string QueryStringSafeHash(string hash)
        {
            hash = hash.Replace('+', '-');
            return hash.Replace('/', '_');
        }
    }
}
