namespace ManagerServer.Services
{
    public sealed class SmtpSettings
    {
        public string Host { get; }
        public int Port { get; }
        public string Username { get; }
        public string Password { get; }
        public bool UseSsl { get; }
        public string FromAddress { get; }

        public SmtpSettings(Uri uri)
        {
            Host = uri.Host;
            Port = uri.Port > 0 ? uri.Port : (uri.Scheme == "smtps" ? 465 : 587);
            UseSsl = uri.Scheme == "smtps";
            if (!string.IsNullOrEmpty(uri.UserInfo))
            {
                var parts = uri.UserInfo.Split(':', 2);
                Username = Uri.UnescapeDataString(parts[0]);
                Password = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : null;
            }
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            FromAddress = query["from"] ?? Username;
        }
    }
}
