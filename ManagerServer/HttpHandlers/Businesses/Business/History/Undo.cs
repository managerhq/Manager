/*
using System.Diagnostics;
using System.Threading.Tasks;
using ManagerServer;
using ManagerServer.Globalization;
using ManagerServer.Orm;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    [Title(nameof(Strings.Undo))]
    [Guide("The **Undo** function allows you to revert all recent changes made to your business data, providing a way to recover from unintended modifications or errors.")]
    [Guide("When you use the **Undo** function, it will create a backup file containing your data with all recent changes rolled back to their previous state.")]
    [Header("How It Works")]
    [Guide("The system maintains a history of all changes made to your data. When you click the **Undo** button, the system will:")]
    [Guide("• Review all recorded changes in reverse chronological order")]
    [Guide("• Remove newly created records that were added")]
    [Guide("• Restore modified records to their previous values")]
    [Guide("• Generate a backup file that you can download and use to replace your current data file")]
    [Header("Important Considerations")]
    [Guide("This operation will undo ALL changes made since the last backup point, not just the most recent change. Make sure you want to revert all modifications before proceeding.")]
    [Guide("After using **Undo**, you will receive a downloadable backup file. You must manually replace your current data file with this backup to complete the undo process.")]
    internal sealed class Undo : BusinessTemplate
    {
        protected override void InnerGet2()
        {
            using (Form(method: "POST"))
            {
                InputSubmit(@class: "btn btn-danger", style: "font-weight: bold", value: Strings.Undo);
            }
        }

        protected override async Task InnerPost()
        {
            var path = System.IO.Path.Combine(ApplicationData.BusinessesDirectory, FileID + ".manager");
            var temp = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.CreateVersion7().ToString() + ".manager");

            using (var db = new SQLiteConnection(path))
            {
                db.Backup(temp);
            }

            using (var db = new SQLiteConnection(temp))
            {
                db.BeginTransaction();

                var changes = db.Table<ManagerServer.ApplicationData.Change>().OrderByDescending(x => x.Key).ToArray();

                foreach (var e in changes)
                {
                    if (e.IsCreatingChange)
                    {
                        db.Delete<ManagerServer.ApplicationData.Object>(e.Object);
                    }
                    else
                    {
                        db.InsertOrReplace(new ManagerServer.ApplicationData.Object() { Key = e.Object, Timestamp = e.Timestamp, ContentType = e.ContentTypeBefore, Content = e.ContentBefore });
                    }

                    db.Delete<ManagerServer.ApplicationData.Change>(e.Key);
                }

                db.Commit();

                db.Execute("VACUUM");
            }

            Response.ContentType = "application/octet-stream";
            Response.Headers["Content-Disposition"] = "attachment; filename*=UTF-8''" + Uri.EscapeDataString(FileID+".manager");
            using (var fs = System.IO.File.OpenRead(temp))
            {
                Response.ContentLength = fs.Length;
                await fs.CopyToAsync(Response.Body);
            }
        }
    }
}
*/