using ManagerServer.Authentication;
using ManagerServer.Endpoints;
using ManagerServer.Model;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace ManagerServer.Api.Businesses.Business
{
    internal abstract class AuthorizedEndpoint<T> : AuthenticatedEndpoint<T>
    {
        [Description("Name of business. If you are calling API from an extension running within IFRAME, then business does not have to be provided. Manager will automatically inject current business. This is useful when extension doesn't know business name which is typically the case.")]
        [InheritedProtoMember(100)] public string Business { get; set; }

        protected UserPermissions UserPermissions { get; private set; }

        public sealed override T AuthenticatedHandle()
        {
            if (string.IsNullOrWhiteSpace(Business))
            {
                Business = Uri.UnescapeDataString(Context.Request.Headers["Manager-Business"].ToString());
            }

            if (string.IsNullOrWhiteSpace(Business))
            {
                throw new BadRequestException("Business name is required. Provide it as a 'business' query parameter (e.g. ?business=Acme%20Pty%20Ltd) or via the 'Manager-Business' request header.");
            }

            var user = Context.GetManagerUser();
            var appData = GetApplicationData();

            if (!appData.Businesses.Exists(Business))
            {
                throw new BadRequestException($"Business '{Business}' does not exist.");
            }

            UserPermissions = ResolveUserPermissions(user, Business, appData);
            if (UserPermissions == null)
            {
                throw new ForbiddenException($"User '{user.Username}' does not have access to business '{Business}'. Ask an administrator to grant access for this business.");
            }

            var permissionNamespace = GetPermissionNamespace();
            if (permissionNamespace != null && !HasVerbPermission(UserPermissions, permissionNamespace))
            {
                throw new ForbiddenException($"User '{user.Username}' lacks the required permission to perform this operation on business '{Business}'.");
            }

            SetCulture(Business, appData);

            return AuthorizedHandle();
        }

        public virtual T AuthorizedHandle()
        {
            return default;
        }

        protected virtual string GetPermissionNamespace()
        {
            const string newPrefix = "ManagerServer.Api.Businesses.Business";
            const string legacyPrefix = "ManagerServer.HttpHandlers.Businesses.Business";

            var ns = GetType().Namespace ?? string.Empty;
            if (!ns.StartsWith(newPrefix, StringComparison.Ordinal)) return null;
            if (ns.Length == newPrefix.Length) return null;
            return legacyPrefix + ns.Substring(newPrefix.Length);
        }

        private static UserPermissions ResolveUserPermissions(UserRecord user, string business, ApplicationData appData)
        {
            if (Edition.IsDesktop)
            {
                return new UserPermissions { AccessType = Model.Enums.UserPermissionsAccessType.FullAccess };
            }
            if (user.Type == UserType.Administrator)
            {
                return new UserPermissions { AccessType = Model.Enums.UserPermissionsAccessType.FullAccess };
            }
            if (user.Businesses != null && user.Businesses.Contains(business))
            {
                var stored = appData.Businesses.Get(business)?
                    .OfType<UserPermissions>()
                    .OrderBy(x => x.Key)
                    .FirstOrDefault(x => x.Username == user.Username);
                return stored ?? new UserPermissions();
            }
            return null;
        }

        private bool HasVerbPermission(UserPermissions permissions, string @namespace)
        {
            var name = GetType().Name;
            if (name.StartsWith("Get", StringComparison.Ordinal)) return permissions.CanView(@namespace);
            if (name.StartsWith("Query", StringComparison.Ordinal)) return permissions.CanView(@namespace);
            if (name.StartsWith("Post", StringComparison.Ordinal)) return permissions.CanCreate(@namespace);
            if (name.StartsWith("Put", StringComparison.Ordinal)) return permissions.CanUpdate(@namespace);
            if (name.StartsWith("Patch", StringComparison.Ordinal)) return permissions.CanUpdate(@namespace);
            if (name.StartsWith("Delete", StringComparison.Ordinal)) return permissions.CanDelete(@namespace);
            return false;
        }

        protected void SetCulture(string business, ApplicationData appData)
        {
            var culture = new CultureInfo("en");
            if (business != null)
            {
                var database = appData.Businesses.Get(business);
                if (database != null)
                {
                    var dateAndNumberFormat = database.Single<ManagerServer.Model.DateAndNumberFormat>();

                    var shortDatePattern = "yyyy-MM-dd";
                    if (!string.IsNullOrWhiteSpace(dateAndNumberFormat.DateFormat)) shortDatePattern = dateAndNumberFormat.DateFormat;

                    var shortTimePattern = "HH:mm:ss";
                    if (!string.IsNullOrWhiteSpace(dateAndNumberFormat.TimeFormat)) shortTimePattern = dateAndNumberFormat.TimeFormat;

                    culture.DateTimeFormat.Calendar = new GregorianCalendar();
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
                            var numberFormat = Serializer.Deserialize<DateAndNumberFormat.NumberFormatParts>(ms);
                            culture.NumberFormat.NumberDecimalSeparator = numberFormat.DecimalSeparator;
                            culture.NumberFormat.NumberGroupSeparator = numberFormat.GroupSeparator;
                            culture.NumberFormat.NumberGroupSizes = numberFormat.GroupSizes;
                            if (Globalization.Languages.IsRightToLeft() && culture.NumberFormat.NumberGroupSeparator == " ")
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
