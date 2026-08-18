using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithImageColumn<T> : NakedObjectsWithCreateNewAndFormDefaultsButtons<T> where T : ManagerServer.Model.Object, new()
    {
        protected override void InnerGet4(Context context)
        {
            if (Request.Query.ContainsKey("Image"))
            {
                var image = Request.Query["Image"].ToString();
                var image2 = ApplicationData.Businesses.GetImage(Business, new Guid(image));
                if (image2 != null)
                {
                    var dateFormat = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
                    Response.Headers["Cache-Control"] = "public, max-age=31536000";
                    Response.Headers["Date"] = DateTime.UtcNow.ToString(dateFormat, CultureInfo.InvariantCulture);
                    Response.Headers["Expires"] = DateTime.UtcNow.AddYears(1).ToString(dateFormat, CultureInfo.InvariantCulture);
                    Response.ContentType = image2.Item2;
                    Response.Body.WriteAsync(image2.Item1, 0, image2.Item1.Length).GetAwaiter().GetResult();
                }
                return;
            }

            base.InnerGet4(context);
        }

        [Center]
        [Default]
        [MinWidth]
        [HideColumnIfAllEmpty]
        [DoNotCopyToClipboard]
        [Icon("fa-image")]
        [Priority(-400)]
        public string[] GetImage(T[] rows)
        {
            var database = ApplicationData.Businesses.Get(Business);
            return rows.Select(x => GetHtml(x.Key, database.GetImage(x.Key))).ToArray();
        }

        private string GetHtml(Guid key, long? timestamp)
        {
            if (!timestamp.HasValue) return null;
            return $@"<i class=""fas fa-image text-(--muted-foreground)/50 hover:text-(--muted-foreground) cursor-pointer text-base"" onclick=""showImage(this, '{key.ToString()}',{timestamp.Value})""></i>";
        }

        protected override void OnAfterFooter(Context context)
        {
            var httpHandler = (BusinessTemplate)this.MemberwiseClone();
            httpHandler.ContentOnly = true;

            using (Dialog(id: "imagePreviewDialog", @class: "m-auto border border-(--border) shadow-lg focus:outline-none", onclick: "document.getElementById('imagePreview').removeAttribute('src'); this.close()"))
            {
                Img(id: "imagePreview", @class: "max-w-[90vw] max-h-[90vh] object-contain");
            }

            using (Script())
            {
                Write($@"function showImage(e, key, timestamp) {{
document.getElementById('imagePreview').src = '{httpHandler.ToUrl()}&Image='+key+'&Timestamp='+timestamp;
document.getElementById('imagePreviewDialog').showModal();
}}");
            }

            base.OnAfterFooter(context);
        }
    }
}
