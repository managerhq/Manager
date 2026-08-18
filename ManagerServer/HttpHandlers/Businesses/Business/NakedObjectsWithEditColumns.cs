using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [Key("edit-columns")]
    [Title(nameof(Strings.EditColumns))]
    [Guide("Most tabular screens in Manager.io allow to customize which columns should be visible. This is excellent way to customize Manager.io to your business requirements.")]
    [Guide("To adjust which columns are visible, click on the `EditColumns` button located in the bottom-right corner.")]
    [SmallBottomButtonScreenshot(nameof(Strings.EditColumns))]
    [Guide("You will be directed to the `EditColumns` screen, where you can choose the columns you want to be visible. Additionally, you can drag and drop columns to rearrange their display order.")]
    [Guide("Click the `Update` button at the bottom to save your preferences.")]
    [SuccessButtonScreenshot(nameof(Strings.Update))]
    [Guide("Note: When choosing which columns to display, select those that are consistently important for you. Begin with fewer options.")]
    [Guide("This feature seamlessly integrates with custom fields, allowing you to display the contents of your custom fields, in addition to the default columns.")]
    [LinkGuide("For more information see:", typeof(Settings.CustomFields.CustomFields))]
    [Guide("If you prefer different views depending on the situation, the `AdvancedQueries` feature enables you to create multiple layouts tailored to each context. With `AdvancedQueries`, not only can you select specific columns, but you can also filter, sort, and group them according to your needs.")]
    [LinkGuide("For more information see:", typeof(NakedObjectsWithAdvancedQueries))]
    internal abstract class NakedObjectsWithEditColumns<T> : NakedObjectsWithBatchView<T>
    {
        [InheritedProtoMember(250)] public bool EditColumns;

        protected override void InnerGet4(Context context)
        {
            var columns = context.Get<Column[]>();

            var guidAttribute = this.GetType().GetCustomAttribute<GuidAttribute>();
            if (EditColumns && guidAttribute != null)
            {
                CustomInnerGet(columns);
                return;
            }

            var customColumns = guidAttribute != null ? ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.CustomColumns>(guidAttribute.Value)?.Columns : null;

            if (customColumns != null)
            {
                foreach (var e in columns.Where(x => x.Key.HasValue))
                {
                    if (e.Key == new Guid("e86f12dd-2bfc-4eef-a7b0-71e4e9caeda9")) continue; // Attachment column never set hidden
                    e.Visible = false;
                }
                for (int i = 0; i < customColumns.Length; i++)
                {
                    var customColumn = customColumns[i];
                    var column = columns.FirstOrDefault(x => x.Key == customColumn.Key);
                    if (column != null)
                    {
                        column.Priority = i;
                        column.Visible = customColumn.Enabled;
                    }
                }
                context.Set(columns.OrderBy(x => x.Priority).ToArray());
            }

            base.InnerGet4(context);
        }

        private void CustomInnerGet(Column[] columns)
        {
            using (Div(@class: "card"))
            {
                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "flex justify-between space-x-8 rtl:space-x-reverse"))
                    {
                        using (Div(@class: "flex items-center space-x-6 rtl:space-x-reverse"))
                        {
                            using (Div(@class: "card-title")) Write(Strings.EditColumns);
                        }
                    }
                }

                using (Div(@class: "card-form"))
                {
                    using (Div(id: "v-model-form"))
                    {
                        using (Table())
                        {
                            using (TBody(v_model: "items", @is: "draggable", tag: "tbody", handle: ".handle"))
                            {
                                using (Tr(v_for: "(lineItem, index) in items"))
                                {
                                    using (Td(@class: "handle cursor-move"))
                                    {
                                        using (Div(v_if: $"items.length > 1", style: "display: table; border-collapse: separate; width: 100%"))
                                        {
                                            using (Span(@class: "form-control text-center whitespace-nowrap"))
                                            {
                                                I(@class: "fas fa-arrows-v");
                                            }
                                        }
                                    }
                                    using (Td())
                                    {
                                        using (Label(@class: "form-control flex items-center gap-2"))
                                        {
                                            InputCheckbox(value: "true", @class: "form-check-input", v_model: $"lineItem.Enabled");
                                            using (Span(v_if: "lineItem.Obsolete")) using (Span(@class: "whitespace-nowrap bg-neutral-400 text-white rounded font-semibold px-2.5 py-0.5 rounded")) Write("Obsolete");
                                            using (Span()) Write("{{ lineItem.DisplayName }}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                using (Div(@class: "card-header"))
                {
                    using (Button(@class: "btn btn-success", onclick: "ajaxPost(this)")) Write(Strings.Update);
                }
            }

            Script("resources/vue/vue.js?version=" + typeof(Template).Assembly.GetName().Version.ToString());
            Script("resources/sortable/sortable.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Dependency for VueDraggable
            Script("resources/vuedraggable/vuedraggable.js?version=" + typeof(Template).Assembly.GetName().Version.ToString()); // Required for reordering rows            

            using (Script())
            {
                var customColumns = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.CustomColumns>(this.GetType().GetCustomAttribute<GuidAttribute>().Value)?.Columns;

                var items = new List<CustomColumn>();
                if (customColumns != null)
                {
                    foreach (var e in customColumns)
                    {
                        var column = columns.FirstOrDefault(x => x.Key == e.Key);
                        if (column != null)
                        {
                            if (column.Key == new Guid("e86f12dd-2bfc-4eef-a7b0-71e4e9caeda9")) continue; // Attachment column
                            var isObsolete = column.Attributes.OfType<ObsoleteAttribute>().Any();
                            items.Add(new CustomColumn() { Key = e.Key, DisplayName = column.Label, Enabled = e.Enabled, Obsolete = isObsolete });
                        }
                    }
                    foreach (var e in columns)
                    {
                        var key = e.Key;
                        if (!key.HasValue) continue;
                        if (key == new Guid("e86f12dd-2bfc-4eef-a7b0-71e4e9caeda9")) continue; // Attachment column
                        var item = items.FirstOrDefault(x => x.Key == key.Value);
                        if (item == null)
                        {
                            var isObsolete = e.Attributes.OfType<ObsoleteAttribute>().Any();
                            items.Add(new CustomColumn() { Key = key.Value, DisplayName = e.Label, Enabled = false, Obsolete = isObsolete });
                        }
                    }
                }
                else
                {
                    foreach (var e in columns)
                    {
                        var isObsolete = e.Attributes.OfType<ObsoleteAttribute>().Any();

                        var key = e.Key;
                        if (!key.HasValue) continue;
                        if (key == new Guid("e86f12dd-2bfc-4eef-a7b0-71e4e9caeda9")) continue; // Attachment column
                        items.Add(new CustomColumn() { Key = key.Value, DisplayName = e.Label, Enabled = e.Visible, Obsolete = isObsolete });
                    }
                }

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(new { items = items.ToArray() });
                Write($@"app = new Vue({{ el: ""#v-model-form"", data: {json} }})");
            }

            using (Script())
            {
                Write(@"function ajaxPost(e) {
    var xhr = new XMLHttpRequest();
    var formData = new FormData();
    formData.append('Json', JSON.stringify(app.$data.items));
    xhr.open('POST', window.location.href, true);
    xhr.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                window.location.href = this.responseText;
            }
            else {
                //enableButtons();
                //Swal.fire(this.responseText);
            }
        }
    }
    xhr.send(formData);
}");
            }
        }

        protected override async Task InnerPost()
        {
            if (EditColumns)
            {
                if (Request.HasFormContentType)
                {
                    var form = await Request.ReadFormAsync();
                    var json = form["Json"];
                    var customColumns = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomColumn[]>(json);

                    var key = this.GetType().GetCustomAttribute<GuidAttribute>().Value;
                    if (customColumns.Any(x => x.Enabled))
                    {
                        var o = ApplicationData.Businesses.Get(Business).SingleOrDefault<ManagerServer.Model.CustomColumns>(key) ?? new ManagerServer.Model.CustomColumns() { Key = key };
                        o.Columns = customColumns.Select(x => new ManagerServer.Model.CustomColumns.CustomColumn() { Key = x.Key, Enabled = x.Enabled }).ToArray();
                        ApplicationData.Businesses.Process(Business, o, GetUserName());
                    }
                    else
                    {
                        ApplicationData.Businesses.Process(Business, key, GetUserName());
                    }

                    var redirect = (NakedObjectsWithEditColumns<T>)this.MemberwiseClone();
                    redirect.EditColumns = false;
                    Write(redirect.ToUrl());
                    return;
                }
            }

            await base.InnerPost();
        }

        public sealed class ObsoleteAttribute : Attribute { }

        public sealed class CustomColumn
        {
            public Guid Key;
            public string DisplayName;
            public bool Enabled;
            public bool Obsolete;
        }

        protected override void OnFooterEndSection(Context context)
        {
            if (!AdvancedSearch.HasValue) // Advanced Search has its own edit columns
            {
                var guidAttribute = this.GetType().GetCustomAttribute<GuidAttribute>();

                if (!EditColumns && guidAttribute != null)
                {
                    var editColumnsHandler = (NakedObjectsWithEditColumns<T>)this.MemberwiseClone();
                    editColumnsHandler.EditColumns = true;
                    editColumnsHandler.Term = null;
                    editColumnsHandler.Skip = 0;
                    editColumnsHandler.PageSize = null;
                    using (A(href: editColumnsHandler.ToUrl(), @class: "btn btn-xs")) Write(Strings.EditColumns);
                }
            }

            base.OnFooterEndSection(context);
        }
    }
}
