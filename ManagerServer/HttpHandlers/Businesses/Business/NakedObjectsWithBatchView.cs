using System;
using System.Linq;
using System.Collections.Generic;
using ManagerServer.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithBatchView<T> : NakedObjectsWithAbstractViewButtonColumn<T>
    {
        [InheritedProtoMember(200)] public bool BatchView;

        protected override void InnerGet4(Context context)
        {
            if (BatchView)
            {
                if (Request.HasFormContentType)
                {
                    var form = Request.ReadFormAsync().GetAwaiter().GetResult();
                    if (form.ContainsKey("BatchView"))
                    {
                        var item = form["BatchView"].ToString();
                        if (!string.IsNullOrWhiteSpace(item))
                        {   
                            var items = item.Split(',').Select(x => Convert.FromBase64String(x)).ToArray();
                            BatchViewScreen(items.Select(x => Encoding.UTF8.GetString(x)).ToArray());
                        }
                    }
                    return;
                }
                else
                {
                    var cancelHandler = (NakedObjectsWithBatchView<T>)this.MemberwiseClone();
                    cancelHandler.BatchView = false;

                    context.Set(new BatchOperation()
                    {
                        Name = Strings.BatchView,
                        Cancel = cancelHandler
                    });
                }
            }
            base.InnerGet4(context);
        }

        public override Tuple<string, byte[]>[] GetBatchOperation(T[] rows)
        {
            if (BatchView)
            {
                var list = new List<Tuple<string, byte[]>>();
                foreach (var e in GetView(rows))
                {
                    if (e != null)
                    {
                        if (e is BaseView3 baseView3)
                        {
                            baseView3.HttpContext = this.HttpContext;
                            var url = baseView3.GetIframeUrl();
                            var data = Encoding.UTF8.GetBytes(url);
                            list.Add(new Tuple<string, byte[]>("BatchView", data));
                        }
                        else
                        {
                            e.ContentOnly = true;
                            e.Referrer = null;
                            var url = e.ToUrl();
                            var data = Encoding.UTF8.GetBytes(url);
                            list.Add(new Tuple<string, byte[]>("BatchView", data));
                        }
                    }
                    else
                    {
                        list.Add(null);
                    }
                }
                return list.ToArray();
            }
            return base.GetBatchOperation(rows);
        }

        protected override void OnFooterEndSection(Context context)
        {
            var batchOperations = GetBatchOperations(context);

            var batchViewHandler = (NakedObjectsWithBatchView<T>)this.MemberwiseClone();
            batchViewHandler.BatchView = true;

            if (batchOperations.Items.Any()) batchOperations.Items.Add(null);
            batchOperations.Items.Add(new Tuple<string, BusinessTemplate>(Strings.BatchView, batchViewHandler));

            base.OnFooterEndSection(context);
        }

        protected override async Task InnerPost()
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (form.ContainsKey("BatchView"))
                {
                    var item = form["BatchView"].ToString();
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        await Get();
                        return;
                    }
                }
            }
            await base.InnerPost();
        }

        private void BatchViewScreen(string[] urls)
        {
            using (Div(@class: "card"))
            {
                using (Div(@class: "card-header flex gap-4 items-center print:hidden"))
                {
                    using (Div(@class: "card-title")) Write(GetTitle());
                    using (A(@class: "btn", href: "javascript:print()")) Write(Strings.Print);
                    using (A(@class: "btn", href: this.ToUrl())) Write(Strings.Cancel);
                }

                using (Div(@class: "card-inset flex flex-col gap-4 p-0"))
                {
                    foreach (var e in urls)
                    {
                        using (IFrame(src: e, onload: "autoResizeIframe(this)", loading: "lazy"))
                        {
                        }
                    }
                }
            }
        }
    }
}