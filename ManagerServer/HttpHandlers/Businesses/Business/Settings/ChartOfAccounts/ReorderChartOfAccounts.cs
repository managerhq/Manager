using System.Collections.Generic;
using System.Linq;
using ManagerServer.Globalization;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.ChartOfAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.ChartOfAccounts), nameof(Strings.Position))]
    [Guide("The *chart of accounts* is the foundation of your accounting system. This screen allows you to reorganize how accounts appear in your financial reports.")]
    [Guide("Use the drag-and-drop interface to reorder accounts within their respective groups. Simply click and hold the drag handle (represented by vertical arrows) next to an account name, then move it to your desired position.")]
    [Guide("Changes you make here will be reflected immediately in all financial reports, including the **Balance Sheet**, **Profit and Loss Statement**, and other accounting reports.")]
    [Guide("Account positions determine the order in which they appear in reports. Accounts at the top of the list will appear first in their respective sections.")]
    internal sealed class ReorderChartOfAccounts : BusinessTemplate
    {
        [ProtoMember(1)] public Guid Key;

        protected override void InnerGet2()
        {
            Script("resources/vue/vue.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
            Script("resources/sortable/sortable.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Dependency for VueDraggable
            Script("resources/vuedraggable/vuedraggable.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Required for reordering rows

            var model = new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel(Business);

            var items = GetItems(model.ProfitAndLossStatement.OfType<ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item>().ToArray(), Key) ?? GetItems(model.BalanceSheet.OfType<ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item>().ToArray(), Key) ?? new ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item[0];
            items.Where(x => x.Key != ManagerServer.Model.Master.AccountKeys.Suspense).ToArray();

            using (Div(id: "v-model-form"))
            {
                using (PostForm())
                {
                    using (Div(@class: "card"))
                    {
                        using (Div(@class: "card-header"))
                        {
                            using (Div(@class: "card-title"))
                            {
                                Write(Strings.ChartOfAccounts);
                            }
                        }

                        using (Div(@class: "card-form"))
                        {
                            using (Table(@class: "border-separate"))
                            {
                                using (TBody(v_model: "items", @is: "draggable", tag: "tbody", handle: ".handle"))
                                {
                                    using (Tr(v_for: "(lineItem, index) in items"))
                                    {
                                        using (Td(@class: "handle cursor-move"))
                                        {
                                            using (Div(@class: "card"))
                                            {
                                                using (Div(@class: "card-header flex items-center gap-4"))
                                                {
                                                    I(@class: "fas fa-arrows-v text-neutral-300");
                                                    using (Span()) Write("{{ lineItem.Item2 }}");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        using (Div(@class: "card-header"))
                        {
                            InputHidden(name: "Json", v_model: "JSON.stringify(items, null, 2)");
                            using (Div()) using (SuccessButton()) Write(Strings.Update);
                        }
                    }                    
                }

#if DEBUG
                using (Pre(@class: "mt-8")) Write("{{ JSON.stringify($data.items, null, 2) }}");
#endif
            }

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { items = items.Select(x => new Tuple<Guid, string>(x.Key, x.NameWithCode)).ToArray() });

            using (Script()) Write($@"app = new Vue({{ el: ""#v-model-form"", data: {json} }})");
        }

        public ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item[] GetItems(ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Item[] items, Guid key)
        {
            if (items.Any(x => x.Key == key)) return items;

            foreach (var e in items.OfType<ManagerServer.Query.GeneralLedger.ChartOfAccountsModel.Group>())
            {
                var items2 = GetItems(e.Items.ToArray(), key);
                if (items2 != null) return items2;
            }

            return null;
        }

        public sealed class FormData
        {
            public Item[] Items;
        }

        public sealed class Item
        {
            public Guid Key;
        }

        protected override async Task InnerPost()
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!userPermissions.CanUpdate(this.GetType().Namespace)) return;

            var form = await Request.ReadFormAsync();
            var json = form["Json"];
            if (!string.IsNullOrWhiteSpace(json))
            {
                var items = JsonConvert.DeserializeObject<Tuple<Guid, string>[]>(json).ToArray();
                var list = new List<ManagerServer.Model.Object>();
                for (int i = 0; i < items.Length; i++)
                {
                    var position = i + 1;
                    var key = items[i].Item1;

                    var o = ApplicationData.Businesses.Get(Business).SingleOrDefault(key) ?? ApplicationData.Businesses.Get(Business).Single(key);
                    if (o != null)
                    {
                        var field = o.GetType().GetFieldOrProperty("Position");
                        if (field != null)
                        {
                            field.SetMemberValue(o, position);
                            list.Add(o);
                        }
                    }
                }
                ApplicationData.Businesses.Process(Business, list.ToArray(), GetUserName());
            }

            Response.Redirect(new ChartOfAccounts() { Business = Business }.ToUrl());
        }
    }
}
