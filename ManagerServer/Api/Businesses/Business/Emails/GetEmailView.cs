using ManagerServer.Globalization;
using ManagerServer.Helpers;

namespace ManagerServer.Api.Businesses.Business.Emails
{
    [ProtoContract]
    internal sealed class GetEmailView : ViewEndpoint<View>, IView
    {
        public override View AuthorizedHandle()
        {
            ManagerServer.ApplicationData.Email email;
            using (var c = GetApplicationData().Businesses.SQLiteConnection(Business))
            {
                email = c.Get<ManagerServer.ApplicationData.Email>(Key);
            }
            if (email == null) return null;

            Languages.SetLanguage(Language);

            var view = new View { Title = email.Subject ?? string.Empty };

            view.Fields.Add(new View.FieldInfo { Label = Strings.From, Text = email.Sender });
            view.Fields.Add(new View.FieldInfo { Label = Strings.To, Text = email.Recipient });
            view.Fields.Add(new View.FieldInfo
            {
                Label = Strings.Date,
                Text = new DateTime(email.Timestamp, DateTimeKind.Utc).ToLocalTime().ToString()
            });

            if (!string.IsNullOrWhiteSpace(email.Body))
            {
                var body = email.Body.Replace("<button ", @"<button onclick=""document.getElementById('content').style.display = 'block'"" ");
                view.Footers.Add("<hr>" + body);
            }

            if (!string.IsNullOrWhiteSpace(email.Content))
            {
                view.Footers.Add(email.Content);
            }

            var businessDetails = GetApplicationData().Businesses.Get(Business).Single<Model.BusinessDetails>();
            view.BusinessName = businessDetails.Name;
            if (string.IsNullOrWhiteSpace(view.BusinessName)) view.BusinessName = Business;
            view.Direction = Languages.IsRightToLeft() ? Direction.Rtl : Direction.Ltr;
            view.Language = Languages.GetLanguage();

            return view;
        }

        public View GetView() => AuthenticatedHandle();
    }
}
