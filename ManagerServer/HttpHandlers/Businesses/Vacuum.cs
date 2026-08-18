using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

using static ManagerServer.ApplicationData;
using System.Text;
using ManagerComponents;
using ManagerServer.Orm;

namespace ManagerServer.HttpHandlers.Businesses
{
    [ProtoContract]
    [Title(nameof(Strings.Vacuum))]
    [Guide("The `Vacuum` function rebuilds your database file to minimize disk space usage.")]
    [Guide("This function is particularly useful when attachments have been added and then deleted from your database. When you delete attachments, the database file doesn't automatically shrink - instead, the freed space remains available for future use.")]
    [Header("How to Use Vacuum")]
    [Guide("To optimize your database and reclaim unused disk space:")]
    [Guide("1. Go to the `Businesses` tab.")]
    [Guide("2. Locate your business and check its disk space usage. Click on the disk space figure to open the `Vacuum` screen.")]
    [Guide("3. On the `Vacuum` screen, click the `Vacuum` button to start the optimization process.")]
    [Header("Important Notes")]
    [Guide("The vacuum operation duration depends on your database size. It can take anywhere from a few seconds to several minutes to complete.")]
    [Guide("The `Vacuum` function will fail if the database is corrupted and cannot be rebuilt. In such cases, the database must be repaired before vacuum can be performed.")]
    [LinkGuide("For more information about database corruption, see:", typeof(Corrupt))]
    internal sealed class Vacuum : Template
    {
        [ProtoMember(1)] public string Business;

        protected override Task InnerGet()
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

                            try
                            {
                                using (var db = ApplicationData.Businesses.SQLiteConnection(Business))
                                {
                                    var pageSize = db.ExecuteScalar<long>("PRAGMA page_size");
                                    var pageCount = db.ExecuteScalar<long>("PRAGMA page_count") * pageSize;
                                    var pageFreeList = db.ExecuteScalar<long>("PRAGMA freelist_count") * pageSize;

                                    using (Table())
                                    {
                                        using (Tr())
                                        {
                                            using (Td()) Write("Database size");
                                            using (Td()) Write(FormatSize(pageCount));
                                        }
                                        using (Tr())
                                        {
                                            using (Td()) Write("Used space");
                                            using (Td()) Write(FormatSize(pageCount - pageFreeList));
                                        }
                                        using (Tr())
                                        {
                                            using (Td()) Write("Free space");
                                            using (Td()) Write(FormatSize(pageFreeList));
                                        }
                                    }
                                }

                                using (Div()) Write("This action will rebuild the database file, repacking it into a minimal amount of disk space.");

                                Hr();

                                FormSuccessButton(nameof(Strings.Vacuum));
                            }
                            catch (SQLiteException ex)
                            {
                                using (Div()) Write(ex.Message);
                            }
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

        static string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        protected override Task InnerPost()
        {
            if (ApplicationData.Businesses.Exists(Business))
            {
                using (var db = ApplicationData.Businesses.SQLiteConnection(Business))
                {
                    db.Pragma("auto_vacuum = INCREMENTAL");
                    db.Vacuum();
                }
            }

            Response.Redirect(this.ToUrl());

            return Task.CompletedTask;
        }
    }
}
