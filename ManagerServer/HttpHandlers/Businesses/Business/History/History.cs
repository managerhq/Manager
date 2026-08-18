using System.Linq;
using ManagerServer;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    [Title(nameof(Strings.History))]
    [Guide("The **History** screen displays all modifications made to your business data. Every change is tracked and recorded here for audit purposes, providing a complete trail of who changed what and when.")]
    [Guide("To access the History screen, click the **History** button located in the top-right corner after opening your business.")]
    [DefaultButtonScreenshot(nameof(Strings.History))]
    [Header("Understanding the History Display")]
    [Guide("The History screen shows a chronological list of all changes made to your business data. Each row represents a single modification and includes detailed information about what was changed.")]
    [Guide("You can use the **View** button on any row to see the complete details of that specific change, including the exact values that were modified.")]
    [Header("Filtering History Entries")]
    [Guide("To find specific changes quickly, use the dropdown filters in the top-right corner of the screen:")]
    [Guide("• **User** - Filter by the person who made the changes")]
    [Guide("• **Type** - Filter by the type of record that was modified (such as invoices, customers, or accounts)")]
    [Guide("• **Action** - Filter by the type of modification (**Create** for new records, **Update** for changes, or **Delete** for removals)")]
    [Header("History and Backups")]
    [Guide("When you create a backup of your business, the history data is included by default. This ensures you maintain a complete audit trail when restoring from a backup.")]
    [Guide("If you need to reduce the backup file size, you can choose to exclude history data during the backup process. However, this means you will lose the audit trail for the excluded period.")]
    [LinkGuide("To learn more about backup options, see:", typeof(Backup))]
    internal sealed class History : NakedObjectsWithPagination
    {
        [ProtoMember(1)] public string User;
        [ProtoMember(2)] public Guid? Object;
        [ProtoMember(3)] public int? Action;
        [ProtoMember(4)] public Guid? ContentType;

        protected override void InnerGet4(Context context)
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!userPermissions.FullAccess) return;

            using (var c = ApplicationData.Businesses.SQLiteConnection(Business))
            {
                var query = c.Table<ManagerServer.ApplicationData.Change>();
                if (!string.IsNullOrWhiteSpace(User))
                {
                    query = query.Where(x => x.User == User);
                }
                else
                {
#if !DEBUG
                    query = query.Where(x => x.User != null);
#endif
                }
                if (ContentType.HasValue) query = query.Where(x => x.ContentTypeBefore == ContentType.Value || x.ContentTypeAfter == ContentType.Value);
                if (Object.HasValue) query = query.Where(x => x.Object == Object.Value);
                if (Action == 0)
                {
                    query = query.Where(x => x.ContentTypeBefore == Guid.Empty);
                    query = query.Where(x => x.ContentTypeAfter != Guid.Empty);
                }
                if (Action == 1)
                {
                    query = query.Where(x => x.ContentTypeBefore != Guid.Empty);
                    query = query.Where(x => x.ContentTypeAfter != Guid.Empty);
                }
                if (Action == 2)
                {
                    query = query.Where(x => x.ContentTypeBefore != Guid.Empty);
                    query = query.Where(x => x.ContentTypeAfter == Guid.Empty);
                }
                context.Set(new Total() { Value = query.Count() });
                var rows = query.OrderByDescending(x => x.Timestamp).Skip(Skip).Take(GetPageSize()).ToArray();
                context.Set<Array>(rows);
            }

            base.InnerGet4(context);
        }

        protected override void OnAfterHeader(Context context)
        {
            using (Style()) Write("s { color: #999 }");
            base.OnAfterHeader(context);
        }

        [Center]
        [Default]
        [MinWidth]
        [Icon("fa-eye")]
        [Guide("The **View** button allows you to examine the complete details of any change. Clicking it opens a detailed view showing exactly what values were modified, making it easy to understand the nature of each change.")]
        public BusinessTemplate[] GetView(ApplicationData.Change[] rows)
        {
            var referrer = this.ToUrl();
            return rows.Select(x => (BusinessTemplate)new HistoryView() { Business = Business, Referrer = referrer, Key = x.Commit }).ToArray();
        }

        [Center]
        [Default]
        [MinWidth]
        [WhitespaceNoWrap]
        [Guide("The **Timestamp** column shows the exact date and time when each modification occurred. All times are displayed in your local timezone for easy reference.")]
        public DateTime[] GetTimestamp(ApplicationData.Change[] rows)
        {
            return rows.Select(x => new DateTime(x.Timestamp, DateTimeKind.Utc)).ToArray();
        }

        [Default]
        [Guide("The **User** column identifies who made each modification. This accountability feature helps you track which user was responsible for each change in your business data.")]
        public string[] GetUser(ApplicationData.Change[] rows)
        {
            return rows.Select(x => x.User).ToArray();
        }

        [Default]
        [Guide("The **Description** column provides context about what was changed. It displays identifying information such as invoice numbers, customer names, or account names, helping you quickly understand which specific record was modified.")]
        public string[] GetDescription(ApplicationData.Change[] rows)
        {
            return rows.Select(x => x.GetDescription()).ToArray();
        }

        [Center]
        [Default]
        [MinWidth]
        [Guide("The **Action** column indicates the type of modification that occurred. There are three types of actions:")]
        [Guide("• **Create** - A new record was added to the system")]
        [Guide("• **Update** - An existing record was modified")]
        [Guide("• **Delete** - A record was removed from the system")]
        public ApplicationData.ChangeType[] GetAction(ApplicationData.Change[] rows)
        {
            return rows.Select(x => x.GetAction()).ToArray();
        }

        protected override void OnHeaderEndSection(Context context)
        {
            using (var c = ApplicationData.Businesses.SQLiteConnection(Business))
            {
                var users = new System.Collections.Generic.List<DistinctUser>();
                if (Object.HasValue) users = c.Query<DistinctUser>("SELECT DISTINCT User FROM Changes WHERE Object = ?", Object.Value.ToString()); // sqlite-net does not support Distinct LINQ method
                else users = c.Query<DistinctUser>("SELECT DISTINCT User FROM Changes"); // sqlite-net does not support Distinct LINQ method

                using (Div())
                {
                    using (Select(@class: "form-select", onchange: "window.location = this.value"))
                    {
                        var emptyHttpHandler = (History)this.MemberwiseClone();
                        emptyHttpHandler.User = null;
                        emptyHttpHandler.Skip = 0;
                        Option(value: emptyHttpHandler.ToUrl());

                        foreach (var e in users.Where(x => !string.IsNullOrWhiteSpace(x.User)).OrderBy(x => x.User))
                        {
                            var httpHandler = (History)this.MemberwiseClone();
                            httpHandler.User = e.User;
                            httpHandler.Skip = 0;

                            Option(value: httpHandler.ToUrl(), text: e.User, selected: e.User == User);
                        }
                    }
                }

                var contentTypes = new System.Collections.Generic.List<Type>();

                var contentTypesBefore = c.Query<DistinctContentTypeBefore>("SELECT DISTINCT ContentTypeBefore FROM Changes WHERE ContentTypeBefore is not NULL");
                foreach (var e in contentTypesBefore)
                {
                    if (Guid.TryParse(e.ContentTypeBefore, out Guid result2))
                    {
                        var type = ManagerServer.Model.Object.GetTypeByGuid(result2);
                        if (type != null && type.Namespace == "ManagerServer.Model")
                        {
                            contentTypes.Add(type);
                        }
                    }
                }

                var contentTypesAfter = c.Query<DistinctContentTypeAfter>("SELECT DISTINCT ContentTypeAfter FROM Changes WHERE ContentTypeAfter is not NULL");
                foreach (var e in contentTypesAfter)
                {
                    if (Guid.TryParse(e.ContentTypeAfter, out Guid result2))
                    {
                        var type = ManagerServer.Model.Object.GetTypeByGuid(result2);
                        if (type != null && type.Namespace == "ManagerServer.Model")
                        {
                            contentTypes.Add(type);
                        }
                    }
                }

                contentTypes = contentTypes.Distinct().ToList();

                using (Div())
                {
                    using (Select(@class: "form-select", onchange: "window.location = this.value"))
                    {
                        var emptyHttpHandler = (History)this.MemberwiseClone();
                        emptyHttpHandler.ContentType = null;
                        emptyHttpHandler.Skip = 0;
                        Option(value: emptyHttpHandler.ToUrl());

                        foreach (var e in contentTypes.OrderBy(x => ManagerServer.Globalization.Strings.GetPropertyValue(x.Name)))
                        {
#if !DEBUG
                            if (e == typeof(ManagerServer.Model.Schema)) continue;
#endif

                            var key = ManagerServer.Model.Object.GetGuidByType(e);

                            var httpHandler = (History)this.MemberwiseClone();
                            httpHandler.ContentType = key;
                            httpHandler.Skip = 0;

                            Option(value: httpHandler.ToUrl(), text: ManagerServer.Globalization.Strings.GetPropertyValue(e.Name), selected: ContentType == key);
                        }
                    }
                }
            }

            using (Div())
            {
                using (Select(@class: "form-select", onchange: "window.location = this.value"))
                {
                    var emptyHttpHandler = (History)this.MemberwiseClone();
                    emptyHttpHandler.Action = null;
                    emptyHttpHandler.Skip = 0;
                    Option(value: emptyHttpHandler.ToUrl());

                    foreach (var e in new[] { ManagerServer.ApplicationData.ChangeType.Create, ManagerServer.ApplicationData.ChangeType.Update, ManagerServer.ApplicationData.ChangeType.Delete })
                    {
                        var httpHandler = (History)this.MemberwiseClone();
                        httpHandler.Action = (int)e;
                        httpHandler.Skip = 0;

                        Option(value: httpHandler.ToUrl(), text: ManagerServer.Globalization.Strings.GetPropertyValue(e.ToString()), selected: (int)e == Action);
                    }
                }
            }
        }

        public sealed class DistinctUser
        {
            public string User { get; set; }
        }

        public sealed class DistinctContentTypeBefore
        {
            public string ContentTypeBefore { get; set; }
        }

        public sealed class DistinctContentTypeAfter
        {
            public string ContentTypeAfter { get; set; }
        }
    }
}
