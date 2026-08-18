using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Attachments
{
    [ProtoContract]
    [Title(nameof(Strings.Attachments))]
    [Guide("The `Attachments` tab provides a centralized view of all files attached to transactions throughout your business.")]
    [Guide("This screen allows you to manage attachments from one location, making it easy to find, view, and rename files without having to navigate to individual transactions.")]
    [Header("Managing Attachments")]
    [Guide("Click the `Edit` button to rename an attachment. This changes only the display name without affecting the original file.")]
    [Guide("Click the `View` button to open the attachment in your default application for that file type.")]
    [LinkGuide("To learn more about editing attachments, see:", typeof(AttachmentForm))]
    [Header("Table Columns")]
    [Guide("The table shows key information about each attachment, including when it was added, which transaction it belongs to, its filename, and file size:")]
    [Columns]
    internal sealed class Attachments : NakedObjectsWithAutomaticRows<Attachment>
    {
        protected override void InnerGet4(Context context)
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!userPermissions.FullAccess) return;

            /*
            if (Manager.ApplicationData.S3Blobs != null)
            {
                context.Set(new BatchOperation() { Name = Strings.BatchUpdate });
            }
            */

            base.InnerGet4(context);
        }

        /*
        public override Tuple<string, byte[]>[] GetBatchOperation(Attachment[] rows)
        {
            var local = Manager.ApplicationData.GetLocalSha256Blobs(Business);
            return rows.Select(x => x.Sha256 == null || local.Contains(Convert.ToHexStringLower(x.Sha256)) ? new Tuple<string, byte[]>(nameof(Attachments), x.Key.ToByteArray()) : null).ToArray();
        }
        */

        [Default]
        [MinWidth, WhitespaceNoWrap, Center]
        [Guid("700e74a2-7ed5-493a-8201-9d06ea636157")]
        public DateTime[] GetDate(Attachment[] rows) => rows.Select(x => x.Date).ToArray();

        [Default]
        [Guid("88ec3de4-7a00-4fe3-9cca-d7ed297c7674")]
        public string[] GetName(Attachment[] rows) => rows.Select(x => x.Name).ToArray();

        [Default]
        [MinWidth, WhitespaceNoWrap, Center]
        [Guid("fad7a852-1fc7-4f05-afda-0b0bb3ec6224")]
        public int[] GetSize(Attachment[] rows) => rows.Select(x => x.Size).ToArray();

        /*
        [Default]
        [MinWidth, WhitespaceNoWrap, Center]
        [Guid("3aa7cf22-b372-4a56-9678-0d8815c601c5")]
        public StorageType[] GetStorage(Attachment[] rows)
        {
            var local = Manager.ApplicationData.GetLocalSha256Blobs(Business);
            return rows.Select(x => x.Sha256 == null || local.Contains(Convert.ToHexStringLower(x.Sha256)) ? StorageType.InDatabase : StorageType.InCloud).ToArray();
        }

        public enum StorageType
        {
            [Primary] InCloud,
            [Success] InDatabase,
        }

        protected override async Task InnerPost()
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (form.ContainsKey(nameof(Attachments)))
                {
                    var item = form[nameof(Attachments)].ToString();
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var keys = item.Split(',').Select(x => Convert.FromBase64String(x)).ToArray();

                        var database = Manager.ApplicationData.Businesses.Get(Business);
                        var local = Manager.ApplicationData.GetLocalSha256Blobs(Business);

                        foreach (var e in keys)
                        {
                            var key2 = new Guid(e);

                            var attachment = database.SingleOrDefault<Attachment>(key2);

                            if (attachment.Sha256 != null)
                            {
                                if (local.Contains(Convert.ToHexStringLower(attachment.Sha256)))
                                {
                                    continue;
                                }
                            }

                            var blob = Manager.ApplicationData.Businesses.GetBlob(Business, key2);
                            if (blob == null) continue;

                            using (var ms = new MemoryStream(blob))
                            {
                                attachment.Sha256 = await Manager.ApplicationData.Attachments.Write(ms);
                            }

                            Manager.ApplicationData.Businesses.Process(Business, new Manager.ApplicationData.Action[] { new CreateOrUpdateAction(attachment) }, GetUserName(), false);
                            Manager.ApplicationData.Businesses.DeleteBlob(Business, key2);
                        });

                        Response.Redirect(this.ToUrl());
                        return;
                    }
                }
            }
            await base.InnerPost();
        }
        */
    }
}
