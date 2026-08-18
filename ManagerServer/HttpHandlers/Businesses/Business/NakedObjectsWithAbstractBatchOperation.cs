using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithAbstractBatchOperation<T> : NakedObjectsWithAdvancedQueries
    {
        protected override void InnerGet4(Context context)
        {
            if (context.Get<BatchOperation>() == null) context.Get<Column[]>().Single(x => x.Priority == -2000).Visible = false;

            base.InnerGet4(context);
        }

        [Center]
        [Default]
        [MinWidth]
        [Priority(-2000)]
        public virtual Tuple<string, byte[]>[] GetBatchOperation(T[] rows)
        {
            return rows.Select(x => default(Tuple<string, byte[]>)).ToArray();
        }

        protected override void OnColumnHeaderCell(Column column)
        {
            if (column.Priority == -2000 || column.Priority == -3000)
            {
                InputCheckbox(onClick: "this.form.querySelectorAll('input[type=checkbox]').forEach(x => x.checked = this.checked)", @class: "form-check-input");
            }
            else
            {
                base.OnColumnHeaderCell(column);
            }
        }

        protected virtual void OnBeforeBatchOperationButton()
        {
        }

        protected override void OnBeforeFooter(Context context)
        {
            var batchOperation = context.Get<BatchOperation>();
            if (batchOperation != null)
            {
                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "flex items-center gap-3"))
                    {
                        I(@class: "fas fa-fw fa-turn-up fa-rotate-90", style: "font-size: 32px; color: #ccc");

                        OnBeforeBatchOperationButton();

                        var buttonClass = "btn btn-primary";
                        if (batchOperation.IsDanger) buttonClass = "btn btn-danger";
                        using (Button(@class: buttonClass)) using (Span(@class: "font-semibold")) Write(batchOperation.Name);
                        if (batchOperation.Cancel != null)
                        {
                            using (A(href: batchOperation.Cancel.ToUrl(), @class: "btn")) Write(Strings.Cancel);
                        }
                    }
                }
            }

            base.OnBeforeFooter(context);
        }

        protected Tuple<string, byte[]>[] Serialize(string name, Array array)
        {
            var output = new Tuple<string, byte[]>[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                var o = array.GetValue(i);
                if (o != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        ProtoBuf.Serializer.Serialize(ms, o);
                        output[i] = new Tuple<string, byte[]>(name, ms.ToArray());
                    }
                }
            }
            return output;
        }

        protected async Task<T2[]> Deserialize<T2>(string name)
        {
            if (!Request.HasFormContentType) return null;
            var form = await Request.ReadFormAsync();
            if (!form.ContainsKey(name)) return null;
            var item = form[name].ToString();
            if (string.IsNullOrWhiteSpace(item)) return null;

            var output = new List<T2>();
            foreach (var e in item.Split(','))
            {
                using (var ms = new MemoryStream(Convert.FromBase64String(e)))
                {
                    output.Add(ProtoBuf.Serializer.Deserialize<T2>(ms));
                }
            }
            return output.ToArray();
        }

        public sealed class BatchOperation
        {
            public string Name;
            public bool IsDanger;
            public HttpHandler Cancel;
        }
    }
}
