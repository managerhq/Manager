using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Model;
using ManagerComponents;
using ManagerServer.Authentication;
using ManagerServer.HttpHandlers.Businesses;
using ManagerServer.HttpHandlers.Businesses.Business.Settings.BusinessDetails;

namespace ManagerServer.HttpHandlers
{
    public abstract class HttpHandler : HttpFramework.HttpHandler
    {
        public override Task ProcessRequest()
        {
            if (this is Status)
            {
                return Get();
            }

            if (this is HttpHandlers.Api.Api || this is HttpHandlers.Api2.Api2)
            {
                switch (Request.Method)
                {
                    case "GET": return Get();
                    case "POST": return Post();
                    case "PUT": return Put();
                    case "DELETE": return Delete();
                    case "OPTIONS": return Options();
                    case "PATCH": return Patch();
                    default: return Task.CompletedTask;
                }
            }

            var currentLanguage = string.Empty;
            if (Request.Cookies["language"] != null)
            {
                currentLanguage = Request.Cookies["language"].ToString();
            }

            ManagerServer.Globalization.Languages.SetLanguage(currentLanguage);

            if (!Edition.IsDesktop)
            {
                if (this is Default || this is LoginTemplate || this is SwitchLanguage || this is Favicon || this is Logo || this is BusinessLogoView)
                {
                    switch (Request.Method)
                    {
                        case "GET": return Get();
                        case "POST": return Post();
                        case "PUT": return Put();
                        case "DELETE": return Delete();
                        case "OPTIONS": return Options();
                        case "PATCH": return Patch();
                        default: return Task.CompletedTask;
                    }
                }
            }

            if (Edition.IsServer)
            {
                var loginRequired = false;
                var user = this.GetCurrentUser();
                if (user == null)
                {
                    loginRequired = true;
                }

                if (loginRequired)
                {
                    if (Request.Path.Value.EndsWith(".json"))
                    {
                        Response.Headers["WWW-Authenticate"] = @"Basic realm = ""Auth""";
                        Response.StatusCode = 401;
                        return Task.CompletedTask;
                    }
                    else
                    {
                        Response.Redirect("/");
                        return Task.CompletedTask;
                    }
                }
            }
            
            switch (Request.Method)
            {
                case "GET": return Get();
                case "POST": return Post();
                case "PUT": return Put();
                case "DELETE": return Delete();
                case "OPTIONS": return Options();
                case "PATCH": return Patch();
                default: return Task.CompletedTask;
            }
        }

        public virtual Task Get() { return Task.CompletedTask; }
        public virtual Task Post() { return Task.CompletedTask; }
        public virtual Task Put() { return Task.CompletedTask; }
        public virtual Task Delete() { return Task.CompletedTask; }
        public virtual Task Options() { return Task.CompletedTask; }
        public virtual Task Patch() { return Task.CompletedTask; }

        internal bool IsAdministrator()
        {
            if (Edition.IsDesktop) return true;
            var user = GetCurrentUser();
            if (user == null) return false;
            return (user.Type == ManagerServer.Model.UserType.Administrator);
        }

        internal ManagerServer.Model.UserPermissions GetCurrentUserPermissions(string fileId)
        {
            if (Edition.IsDesktop) return new ManagerServer.Model.UserPermissions() { AccessType = ManagerServer.Model.Enums.UserPermissionsAccessType.FullAccess };

            var user = GetCurrentUser();
            if (user == null) return new ManagerServer.Model.UserPermissions();
            if (user.Type == ManagerServer.Model.UserType.Administrator) return new ManagerServer.Model.UserPermissions() { AccessType = ManagerServer.Model.Enums.UserPermissionsAccessType.FullAccess };

            if (user.Businesses != null && user.Businesses.Contains(fileId))
            {
                var userPermissions = ApplicationData.Businesses.Get(fileId)?.OfType<ManagerServer.Model.UserPermissions>().OrderBy(x => x.Key).FirstOrDefault(x => x.Username == user.Username);
                if (userPermissions == null) return new ManagerServer.Model.UserPermissions();
                else return userPermissions;
            }

            return new ManagerServer.Model.UserPermissions();
        }

        internal string GetUserName()
        {
            if (Edition.IsDesktop)
            {
                return Environment.UserName;
            }
            else
            {
                var currentUser = GetCurrentUser();
                if (currentUser != null)
                {
                    if (!string.IsNullOrWhiteSpace(currentUser.Name)) return currentUser.Name;
                    return currentUser.Username ?? string.Empty;
                }
                else
                {
                    return string.Empty;
                }                
            }
        }

        internal UserRecord GetCurrentUser()
        {
            return HttpContext.GetManagerUser();
        }

        internal void EnsureCurrentUserNotRestricted()
        {
            var user = GetCurrentUser();
            if (user != null && user.Type == ManagerServer.Model.UserType.Restricted) throw new Exception("Restricted");
        }

        protected void Write(ComponentBase component)
        {
            var sb = new StringBuilder();
            component.BuildString(sb);
            Write(sb);
        }

        protected void SetCulture(string fileId)
        {
            var culture = new System.Globalization.CultureInfo("en");
            if (fileId != null)
            {
                var database = ApplicationData.Businesses.Get(fileId);
                if (database != null)
                {
                    var dateAndNumberFormat = database.Single<ManagerServer.Model.DateAndNumberFormat>();

                    var shortDatePattern = "yyyy-MM-dd";
                    if (!string.IsNullOrWhiteSpace(dateAndNumberFormat.DateFormat)) shortDatePattern = dateAndNumberFormat.DateFormat;

                    var shortTimePattern = "HH:mm:ss";
                    if (!string.IsNullOrWhiteSpace(dateAndNumberFormat.TimeFormat)) shortTimePattern = dateAndNumberFormat.TimeFormat;

                    culture.DateTimeFormat.Calendar = new System.Globalization.GregorianCalendar();
                    culture.DateTimeFormat.LongDatePattern = shortDatePattern;
                    culture.DateTimeFormat.ShortDatePattern = shortDatePattern;
                    culture.DateTimeFormat.ShortTimePattern = shortTimePattern;
                    culture.DateTimeFormat.LongTimePattern = shortTimePattern;                    

                    var firstDayOfTheWeek = (int)dateAndNumberFormat.FirstDayOfWeek;
                    culture.DateTimeFormat.FirstDayOfWeek = (DayOfWeek)firstDayOfTheWeek;

                    if (!string.IsNullOrWhiteSpace(dateAndNumberFormat.NumberFormat))
                    {
                        using (var ms = new System.IO.MemoryStream(Convert.FromBase64String(dateAndNumberFormat.NumberFormat)))
                        {
                            var numberFormat = ProtoBuf.Serializer.Deserialize<ManagerServer.Model.DateAndNumberFormat.NumberFormatParts>(ms);
                            culture.NumberFormat.NumberDecimalSeparator = numberFormat.DecimalSeparator;
                            culture.NumberFormat.NumberGroupSeparator = numberFormat.GroupSeparator;
                            culture.NumberFormat.NumberGroupSizes = numberFormat.GroupSizes;
                            if (ManagerServer.Globalization.Languages.IsRightToLeft() && culture.NumberFormat.NumberGroupSeparator == " ")
                            {
                                culture.NumberFormat.NumberGroupSeparator = string.Empty;
                            }
                        }
                    }
                }
            }
            CultureInfo.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        }
    }
}