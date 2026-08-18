using ManagerServer.Globalization;
using ManagerServer.Attributes;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Formats.Tar;
using ManagerServer.Helpers;
using System.Collections.Generic;
using ManagerServer.HttpHandlers.Businesses.Business.Reports.CustomerStatementsUnpaidInvoices;
using Markdig.Syntax;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    [Title(nameof(Strings.Backup))]
    [Guide("The `Backup` function allows you to create a complete copy of your business data for safekeeping. Regular backups protect against data loss and allow you to restore your business to a previous state if needed.")]
    [Guide("To create a backup, click the `Backup` button in the top-right corner of the business summary screen.")]
    [DefaultButtonScreenshot(nameof(Strings.Backup))]
    [Header("Creating a Backup")]
    [Guide("When creating a backup, the system automatically suggests a filename using your business name and today's date. You can modify this name to suit your needs.")]
    [Guide("The backup process allows you to choose which types of data to include:")]
    [Guide("• `Attachments` - Include all files and documents attached to transactions, such as receipts, invoices, and supporting documentation")]
    [Guide("• `Emails` - Include all emails sent from within the program, maintaining a complete communication history")]
    [Guide("• `History` - Include the complete audit trail showing every change made to your data, who made it, and when")]
    [Guide("All checkboxes are selected by default to ensure a complete backup. Deselect any items you don't need to reduce the backup file size.")]
    [Header("Restoring from Backup")]
    [Guide("Backup files are saved with a `.manager` extension and contain all your business data in a compressed format.")]
    [Guide("To restore a backup, use the `Import Business` function from the main businesses screen. Simply select your backup file and the system will restore all data.")]
    [LinkGuide("For detailed restoration instructions, see:", typeof(ImportBusiness))]
    [Header("Cloud Edition Backup Options")]
    [Guide("If you're using Cloud Edition, you have an additional backup method available through the customer portal.")]
    [Guide("Visit [cloud.manager.io](https://cloud.manager.io) and log in with your credentials to access and download your business backups directly.")]
    [Guide("This portal access remains available even if your Cloud Edition subscription expires, ensuring you can always retrieve your data without additional costs.")]
    [Guide("Backups downloaded from the portal can be imported into any edition of the software, including the free Desktop Edition.")]
    internal sealed class Backup : BusinessTemplate
    {
        protected override void InnerGet2()
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!userPermissions.FullAccess) return;

            using (Form(method: "POST", @class: "card"))
            {
                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "card-title")) Write(Strings.Backup);
                }

                using (Div(@class: "card-form"))
                {
                    using (Div(@class: "flex flex-col gap-2"))
                    {
                        using (Label(@class: "flex items-center gap-2"))
                        {
                            InputCheckbox(name: "Timestamp", @class: "form-check-input");
                            using (Span()) Write(Strings.Timestamp);
                        }

                        using (Label(@class: "flex items-center gap-2"))
                        {
                            InputCheckbox(name: "Attachments", @class: "form-check-input");
                            using (Span()) Write(Strings.Attachments);
                        }
                    }
                }

                using (Div(@class: "card-header"))
                {
                    InputSubmit(@class: "btn btn-primary", style: "font-weight: bold", value: Strings.Backup);
                }

                //using (Div(@class: "text-end text-sm p-2")) using (A(href: new Purge() { Business = Business }.ToUrl())) Write(Strings.Export);
            }
        }

        public sealed class FormData
        {
            public string Name;
        }

        protected override async Task InnerPost()
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!userPermissions.FullAccess)
            {
                Response.StatusCode = 403;
                return;
            }

            var form = await Request.ReadFormAsync();

            var timestamp = form.ContainsKey("Timestamp");
            var attachments = form.ContainsKey("Attachments");

            var extension = "manager";
            if (Whitelabel.IsEnabled) extension = "bak";

            var filename = $"{Business}.{extension}";
            if (timestamp) filename = $"{Business} ({DateTime.Today.ToString("yyyy-MM-dd")}).{extension}";

            if (attachments)
            {
                Response.ContentType = "application/x-tar";
                Response.Headers["Content-Disposition"] = "attachment; filename*=UTF-8''" + Uri.EscapeDataString(filename + ".tar");
                Features.Get<Stopwatch>().Stop();

                await using var tarWriter = new TarWriter(Response.Body, leaveOpen: true);
                var success = await ApplicationData.Businesses.Backup(Business, filename, tarWriter);
                if (!success)
                {
                    Features.Get<Stopwatch>().Start();
                    Response.Redirect(this.ToUrl());
                    return;
                }

                var blobs = new HashSet<string>();
                foreach (var e in ApplicationData.Businesses.Get(Business).OfType<Model.Attachment>())
                {
                    if (e.Sha256 != null)
                    {
                        var hex = Convert.ToHexStringLower(e.Sha256);
                        if (!blobs.Contains(hex))
                        {
                            using var blob = await ApplicationData.Storage.ReadAsync(e.Sha256);
                            if (blob != null)
                            {
                                var blobPath = $"Blobs/{hex[..2]}/{hex}";
                                var entry = new PaxTarEntry(TarEntryType.RegularFile, blobPath) { DataStream = blob };
                                await tarWriter.WriteEntryAsync(entry);
                            }
                            blobs.Add(hex);
                        }
                    }
                }

                Features.Get<Stopwatch>().Start();
            }
            else
            {
                Response.ContentType = "application/x-manager";
                Response.Headers["Content-Disposition"] = "attachment; filename*=UTF-8''" + Uri.EscapeDataString(filename);
                Features.Get<Stopwatch>().Stop();
                var success = await ApplicationData.Businesses.Backup(Business, Response);
                Features.Get<Stopwatch>().Start();
                if (!success)
                {
                    Response.Redirect(this.ToUrl());
                }
            }
        }

    }
}
