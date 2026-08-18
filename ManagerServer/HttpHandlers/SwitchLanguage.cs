using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers
{
    [ProtoContract]
    public sealed class SwitchLanguage : HttpHandler
    {
        [ProtoMember(1)] public string Language;

        public override Task Post()
        {
            var languageCode = "en";
            var language = ManagerServer.Globalization.Languages.GetLanguages().FirstOrDefault(x => x.Code == Language);
            if (language != null) languageCode = language.Code;

            var cookieOptions = new Microsoft.AspNetCore.Http.CookieOptions() {
                Expires = DateTime.UtcNow.AddYears(1),
                HttpOnly = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax
            };

            Response.Cookies.Append("language", languageCode, cookieOptions);

            Response.Headers["HX-Refresh"] = "true";

            return Task.CompletedTask;
        }
    }
}