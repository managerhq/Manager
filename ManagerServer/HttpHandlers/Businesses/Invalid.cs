using System;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ProtoBuf;
using System.IO;
using System.Linq;
using ManagerServer;
using ManagerServer.HttpHandlers.Businesses.Business;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses
{
    [ProtoContract]
    [Title(nameof(Strings.Error))]
    [Guide("This error occurs when Manager cannot open or access a business database file.")]
    [Guide("There are several possible reasons why you might see this error:")]
    [Guide("• The business database file has been moved, renamed, or deleted")]
    [Guide("• You do not have the necessary permissions to access the file")]
    [Guide("• The database file is corrupted or in an incompatible format")]
    [Guide("• The business ID in the URL is incorrect")]
    [Header("What to do")]
    [Guide("If you see this error, try the following steps:")]
    [Guide("1. Verify that the business database file exists in the correct location")]
    [Guide("2. Check that you have read/write permissions for the file")]
    [Guide("3. Return to the main `Businesses` screen and try opening the business again")]
    [Guide("4. If the problem persists, the database file may need to be restored from a backup")]
    internal sealed class Invalid : Template
    {
        [ProtoMember(1)] public string Business;

        protected override async Task InnerGet()
        {
            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        using (Div(@class: "flex flex-col space-y-4"))
                        {
                            using (Div(@class: "text-xl font-bold")) Write(Business);

                            Hr();

                            var legacy = await ApplicationData.Businesses.IsLegacyFormat(Business);

                            if (legacy)
                            {
                                using (Div(@class: "font-semibold")) Write("Database Upgrade Required");
                                using (Div()) Write("You are attempting to open a database in the older proprietary format. As part of our ongoing commitment to improve performance, security, and compatibility, Manager has transitioned to the more robust SQLite database format since 2016.");
                                using (Div()) Write("Your data is still intact and safe, but you'll need to upgrade your database to the new SQLite format to continue using Manager without any interruptions.");
                                Hr();
                                using (Div())
                                {
                                    FormPrimaryButton(nameof(Strings.Upgrade));
                                }
                            }
                            else
                            {
                                using (Div(@class: "font-semibold")) Write("This business database is not valid");
                                using (Div()) Write("Manager data is stored in SQLite database. However it appears the file you are attempting to open is not a SQLite database.");
                                using (Div()) using (DefaultLink(new Businesses().ToUrl())) Write(Strings.Back);
                            }
                        }
                    }
                }
            }
        }

        protected override async Task InnerPost()
        {
            await ApplicationData.Businesses.ConvertFromLegacy(Business);

            Response.Redirect(new Businesses().ToUrl());
        }
    }
}
