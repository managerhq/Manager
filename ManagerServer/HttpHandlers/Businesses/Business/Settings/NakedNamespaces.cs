using System;
using System.Linq;
using System.Reflection;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using System.Text;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings
{
    [NamespaceEntry]
    internal abstract class NakedNamespaces : BusinessTemplate
    {
        private BusinessTemplate[] GetChildren(TabsExtensions.Item[] tabs)
        {
            var types = typeof(NakedNamespaces).Assembly.GetTypes()
                    .Where(x => x.Namespace != null)
                    .Where(x => x.Namespace.StartsWith(GetType().Namespace + "."))
                    .Where(x => x.Namespace.Split('.').Length == GetType().Namespace.Split('.').Length + 1)
                    .Where(x => x.GetCustomAttribute<NamespaceEntryAttribute>() != null)
                    .Select(x => Activator.CreateInstance(x) as BusinessTemplate)
                    .ToList();

            foreach (var e in types)
            {
                e.Business = Business;
                e.HttpContext = HttpContext;
            }

            foreach (var e in types.ToArray())
            {
                foreach (var e2 in e.GetType().GetCustomAttributes<IfTabAttribute>())
                {
                    if (e2.Value.Select(x => tabs.Single(y => y.Name == x)).All(x => !x.Visible))
                    {
                        types.Remove(e);
                        continue;
                    }
                }
            }

            return types.ToArray();
        }

        internal override bool IsEmpty(TabsExtensions.Item[] tabs)
        {
            return GetChildren(tabs).All(x => x.IsEmpty(tabs));
        }

        protected override void InnerGet2()
        {
            var tabs = this.GetTabs(false).GetAll();
            var referrer = this.ToUrl();

            /*
            using (Div(@class: "card"))
            {
                using (Div(@class: "card-header flex gap-2 items-center"))
                {
                    using (Div(@class: "card-title")) Write(Strings.GetPropertyValue(GetTitle()));
                    WriteHelp();
                }
            }
            */

            var userPermissions = GetCurrentUserPermissions(Business);

            var first = true;
            foreach (var e in GetChildren(tabs).GroupBy(x => x.IsEmpty(tabs)).OrderBy(x => x.Key))
            {
                if (first) first = false;
                else Hr(@class: "m-0");

                using (Div(@class: "flex flex-wrap gap-4 p-4"))
                {
                    foreach (var e2 in e.OrderBy(x => Strings.GetPropertyValue(x.GetType().Namespace.Split('.').Last())))
                    {
                        if (!userPermissions.CanView(e2.GetType().Namespace)) continue;

                        var key = e2.GetType().Namespace.Split('.').Last();

                        if (e2.GetType().BaseType.IsGenericType && e2.GetType().BaseType.GetGenericTypeDefinition() == typeof(NakedVueForm<>))
                        {
                            var genericArgument = e2.GetType().BaseType.GetGenericArguments().Single();
                            ((VueForm)e2).Key = ManagerServer.Model.Object.GetGuidByType(genericArgument);
                        }

                        e2.Referrer = referrer;

                        using (A(href: e2.ToUrl(), @class: $"basis-72 flex items-center gap-4 p-4 hover:bg-(--card-foreground)/5 hover:rounded-xl {(e2.IsEmpty(tabs) ? "opacity-50 hover:opacity-100" : null)}"))
                        {
                            I(@class: "text-3xl text-(--card-foreground)/25 fas fa-fw " + Icons.GetIcon(key));
                            using (Span()) Write(Strings.GetPropertyValue(key));
                        }
                    }
                }
            }
        }
    }
}