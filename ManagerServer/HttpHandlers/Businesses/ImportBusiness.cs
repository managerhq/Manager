using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using ManagerServer.Globalization;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ManagerServer.HttpHandlers.Businesses.Business;
using ManagerServer.Attributes;
using Microsoft.AspNetCore.Http;
using ManagerServer.Extensions;

namespace ManagerServer.HttpHandlers.Businesses
{
    [ProtoContract]
    [Title(nameof(Strings.ImportBusiness))]
    [Guide("The `Import Business` function allows you to bring existing business data files into Manager. This is the primary method for restoring backups, transferring businesses between different editions of Manager, or moving your data to a new computer.")]
    [Guide("Import Business works with backup files created using Manager's `Backup` function. These files contain all your business data, including transactions, customers, suppliers, inventory items, and settings.")]
    [LinkGuide("To learn how to create backups:", typeof(Backup))]
    [Header("Common Use Cases")]
    [Guide("**Moving between editions** — Transfer your business from `Desktop Edition` to `Cloud Edition` (or vice versa) by creating a backup in one edition and importing it into the other.")]
    [Guide("**Setting up a new computer** — When switching to a new computer, create a backup on your old computer and import it on the new one to continue working with your existing data.")]
    [Guide("**Restoring from backups** — Recover from data loss, corruption, or accidental deletions by importing a previously saved backup file.")]
    [Guide("**Creating test environments** — Import a backup of your live business to create a separate test environment where you can safely experiment with new features or settings.")]
    [Header("File Compatibility")]
    [Guide("Manager accepts backup files with the `.manager` extension.")]
    [Guide("The import process automatically handles any necessary data conversions when moving between different versions of Manager, ensuring your data remains intact and accessible.")]
    [Header("How to Import a Business")]
    [Guide("To begin the import process, navigate to the `Businesses` tab from the main screen.")]
    [TopLevelTabScreenshot("fa-building", nameof(Strings.Businesses))]
    [Guide("Click the `Add Business` button and select `Import Business` from the dropdown menu.")]
    [Guide("On the import screen, click the file selection button to browse your computer for the backup file you want to import.")]
    [Guide("After selecting your backup file, click the `Import` button to begin the import process.")]
    [PrimaryButtonScreenshot(nameof(Strings.Import))]
    [Guide("The import process may take a few moments depending on the size of your business data. A progress bar will display the import status.")]
    [Guide("Once the import is complete, you'll be returned to the `Businesses` tab. Your newly imported business will appear in the list. Click on it to open the business and verify that all your data has been imported correctly.")]
    [Header("Options")]
    [Guide("Before importing, you can expand the `Options` section to access additional import settings.")]
    [Guide("**Drop History** — Enable this option to remove all edit history from the imported business. This drops the change tracking data, resulting in a smaller file size. This is useful when you want a clean copy of the business without historical records of individual changes.")]
    [LinkGuide("For more information about managing multiple businesses:", typeof(Businesses))]
    internal sealed class ImportBusiness : Template
    {
        [ProtoMember(1)] public string Error;

        protected override Task InnerGet()
        {
            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        var currentUser = this.GetCurrentUser();
                        if (currentUser != null && currentUser.Type == ManagerServer.Model.UserType.Restricted)
                        {
                            using (Div(style: "font-size: 24px; font-weight: bold; color: #333; padding-top: 20px")) Write("You are not authorised");
                            using (Div(style: "font-weight: bold; padding-top: 20px; line-height: 175%")) Write("You are not authorised to access this part of the system. Only administrators of <u>" + Request.Host + "</u> are allowed.");
                        }
                        else
                        {
                            using (Form(id: "form", method: "POST"))
                            {
                                InputHidden(name: "UploadId", id: "UploadId");
                                InputHidden(name: "FileName", id: "FileName");

                                using (Div(@class: "flex flex-col space-y-4"))
                                {
                                    using (Div(@class: "text-xl font-bold")) Write(Strings.ImportBusiness);

                                    Hr();

                                    using (Div())
                                    {
                                        InputFile(name: "File", id: "File", accept: Whitelabel.IsEnabled ? ".bak" : ".manager,.tar", form: "form", @class: "form-file");
                                    }

                                    Hr();

                                    using (Div(@class: "flex gap-4"))
                                    {
                                        using (Button(@class: "btn btn-success", id: "import-btn"))
                                        {
                                            Write(Strings.ImportBusiness);
                                        }
                                        using (A(href: new Businesses().ToUrl(), @class: "btn")) Write(Strings.Cancel);
                                    }

                                    using (Progress(id: "progress", value: "0", max: 100, @class: "hidden")) Write("0%");

                                    if (!string.IsNullOrWhiteSpace(Error))
                                    {
                                        using (Div(@class: "text-red-500 font-semibold"))
                                        {
                                            Write(Error);
                                        }
                                    }
                                }

                                using (Script())
                                {
                                    Write(@"
document.getElementById('form').addEventListener('submit', async function(e) {
    e.preventDefault();
    var fileInput = document.getElementById('File');
    if (!fileInput.files.length) return;
    var file = fileInput.files[0];
    var btn = document.getElementById('import-btn');
    var progress = document.getElementById('progress');
    btn.disabled = true;
    fileInput.disabled = true;
    progress.classList.remove('hidden');
    var chunkSize = 10485760;
    try {
        var startRes = await fetch('/upload/start?fileSize=' + file.size + '&chunkSize=' + chunkSize, { method: 'POST' });
        if (!startRes.ok) throw new Error('Failed to start upload');
        var start = await startRes.json();
        for (var i = 0; i < start.totalChunks; i++) {
            var offset = i * start.chunkSize;
            var chunk = file.slice(offset, offset + start.chunkSize);
            var res = await fetch('/upload/chunk?uploadId=' + start.uploadId + '&chunkIndex=' + i, { method: 'POST', body: chunk });
            if (!res.ok) throw new Error('Failed to upload chunk ' + i);
            progress.value = (i + 1) / start.totalChunks * 100;
        }
        var completeRes = await fetch('/upload/complete?uploadId=' + start.uploadId, { method: 'POST' });
        if (!completeRes.ok) throw new Error('Failed to complete upload');
        document.getElementById('UploadId').value = start.uploadId;
        document.getElementById('FileName').value = file.name;
        fileInput.removeAttribute('name');
        document.getElementById('form').submit();
    } catch (err) {
        alert(err.message);
        btn.disabled = false;
        fileInput.disabled = false;
        progress.classList.add('hidden');
    }
});
");
                                }
                            }
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        protected override async Task InnerPost()
        {
            this.EnsureCurrentUserNotRestricted();

            var form = await Request.ReadFormAsync();
            var uploadIdStr = form["UploadId"].ToString();

            if (!string.IsNullOrEmpty(uploadIdStr) && Guid.TryParse(uploadIdStr, out var uploadId))
            {
                var fileName = form["FileName"].ToString();
                await ProcessUpload(uploadId, fileName);
            }
            else
            {
                if (form.Files.Count == 0)
                {
                    Response.Redirect(new ImportBusiness() { Error = "Please upload valid file" }.ToUrl());
                    return;
                }

                await Process(form.Files[0]);
            }

            Response.Redirect(new Businesses().ToUrl());
        }

        private async Task ProcessUpload(Guid uploadId, string fileName)
        {
            var dir = ResumableUploadExtension.GetUploadDirectory(uploadId.ToString("N"));
            if (dir == null) return;

            var assembledPath = System.IO.Path.Combine(dir, "assembled");
            if (!File.Exists(assembledPath)) return;

            if (string.IsNullOrWhiteSpace(fileName)) return;

            var extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();

            if (extension == ".tar")
            {
                using (var stream = File.OpenRead(assembledPath))
                    await ProcessTar(stream);
                try { File.Delete(assembledPath); } catch { }
                try { Directory.Delete(dir, true); } catch { }
                return;
            }

            if (extension != ".manager" && extension != ".bak") return;

            var nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
            using (var stream = File.OpenRead(assembledPath))
                await ApplicationData.Businesses.ImportStream(nameWithoutExtension, stream);
            try { Directory.Delete(dir, true); } catch { }
        }

        private async Task Process(IFormFile file)
        {
            var nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

            if (extension == ".json")
            {
                using (var ms = new MemoryStream())
                {
                    Features.Get<System.Diagnostics.Stopwatch>().Stop();
                    await file.CopyToAsync(ms);
                    Features.Get<System.Diagnostics.Stopwatch>().Start();
                    await ApplicationData.Businesses.ImportJson(nameWithoutExtension, ms.ToArray());
                }
                return;
            }

            if (extension == ".tar")
            {
                Features.Get<System.Diagnostics.Stopwatch>().Stop();
                await ProcessTar(file.OpenReadStream());
                Features.Get<System.Diagnostics.Stopwatch>().Start();
                return;
            }

            if (extension == ".manager" || extension == ".bak")
            {
                try
                {
                    Features.Get<System.Diagnostics.Stopwatch>().Stop();
                    await ApplicationData.Businesses.ImportStream(nameWithoutExtension, file.OpenReadStream());
                    Features.Get<System.Diagnostics.Stopwatch>().Start();
                }
                catch (BadHttpRequestException)
                {
                }
            }
        }

        private async Task ProcessTar(Stream tarStream)
        {
            await using var bulkWriter = ApplicationData.Storage.CreateBulkWriter();
            await using var tarReader = new TarReader(tarStream);

            TarEntry entry;
            while ((entry = await tarReader.GetNextEntryAsync()) != null)
            {
                if (entry.DataStream == null) continue;

                if (entry.Name.EndsWith(".manager", StringComparison.OrdinalIgnoreCase))
                {
                    var nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(entry.Name);
                    await ApplicationData.Businesses.ImportStream(nameWithoutExtension, entry.DataStream);
                }
                else if (entry.Name.StartsWith("Blobs/", StringComparison.OrdinalIgnoreCase))
                {
                    await bulkWriter.WriteAsync(entry.DataStream);
                }
            }
        }

    }
}
