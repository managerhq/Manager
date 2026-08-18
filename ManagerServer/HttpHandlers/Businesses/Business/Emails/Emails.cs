using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Emails
{
    [ProtoContract]
    [Title(nameof(Strings.Emails))]
    [Guide("The `Emails` tab displays a complete history of all emails sent from Manager. This includes emails generated from sales invoices, purchase orders, statements, and any other documents sent via email.")]
    [Guide("Each email entry shows the recipient's email address, subject line, timestamp, and current delivery status. The status indicates whether the email was successfully sent, is pending, or if there was a delivery failure.")]
    [Guide("To view the full content of an email, click the `View` button next to any email entry. This will display the complete email message along with any attachments that were included.")]
    [Guide("You can filter the email list by user if multiple users have access to the system. Use the dropdown menu at the top of the screen to show emails sent by a specific user or view all emails across all users.")]
    internal sealed class Emails : NakedObjectsWithPagination
    {
        [ProtoMember(1)] public string User;
        [ProtoMember(2)] public Guid? Object;

        protected override void InnerGet4(Context context)
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!userPermissions.FullAccess) return;

            using (var c = ApplicationData.Businesses.SQLiteConnection(Business))
            {
                var query = c.Table<ApplicationData.Email>();
                if (!string.IsNullOrWhiteSpace(User)) query = query.Where(x => x.User == User);
                if (Object.HasValue) query = query.Where(x => x.Object == Object.Value);
                context.Set(new Total() { Value = query.Count() });
                var rows = query.OrderByDescending(x => x.Timestamp).Skip(Skip).Take(GetPageSize()).ToArray();
                context.Set<Array>(rows);
            }

            base.InnerGet4(context);
        }

        protected override void OnHeaderEndSection(Context context)
        {
            using (var c = ApplicationData.Businesses.SQLiteConnection(Business))
            {
                var users = new System.Collections.Generic.List<string>();
                if (Object.HasValue) users = c.Table<ApplicationData.Email>().Where(x => x.Object == Object.Value).Select(x => x.User).Distinct().ToList();
                else users = c.Table<ApplicationData.Email>().Select(x => x.User).Distinct().ToList();

                using (Div())
                {
                    using (Select(@class: "form-select", onchange: "window.location = this.value"))
                    {
                        var emptyHttpHandler = (Emails)this.MemberwiseClone();
                        emptyHttpHandler.User = null;
                        emptyHttpHandler.Skip = 0;
                        Option(value: emptyHttpHandler.ToUrl());

                        foreach (var e in users.Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x))
                        {
                            var httpHandler = (Emails)this.MemberwiseClone();
                            httpHandler.User = e;
                            httpHandler.Skip = 0;

                            Option(value: httpHandler.ToUrl(), text: e, selected: e == User);
                        }
                    }
                }
            }
        }

        [Center]
        [Default]
        [MinWidth]
        [Icon("fa-eye")]
        public BusinessTemplate[] GetView(ManagerServer.ApplicationData.Email[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => new EmailView() { Business = Business, Referrer = referrer, Key = x.Key }).ToArray();
        }

        [Center]
        [Default]
        [MinWidth]
        [WhitespaceNoWrap]
        public DateTime[] GetTimestamp(ManagerServer.ApplicationData.Email[] rows)
        {
            return rows.Select(x => new DateTime(x.Timestamp, DateTimeKind.Utc)).ToArray();
        }

        [Default]
        public string[] GetRecipient(ManagerServer.ApplicationData.Email[] rows)
        {
            return rows.Select(x => x.Recipient).ToArray();
        }

        [Default]
        public string[] GetSubject(ManagerServer.ApplicationData.Email[] rows)
        {
            return rows.Select(x => x.Subject).ToArray();
        }

        [Center]
        [Default]
        [MinWidth]
        public ApplicationData.EmailStatus[] GetStatus(ManagerServer.ApplicationData.Email[] rows)
        {
            return rows.Select(x => x.GetStatus()).ToArray();
        }
    }
}